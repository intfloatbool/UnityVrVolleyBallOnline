using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

namespace VrVolleyball
{
    public class PlayerSportHands : MonoBehaviour
    {
        [Space(5f)]
        [Header("Debugging in editor")]
        [SerializeField] private bool _isDebug = true;
        [SerializeField] private bool _isLeftHandGrabDebug;
        [SerializeField] private bool _isRightHandGrabDebug;
        
        [Space(3f)]
        [SerializeField] private SportHand _leftHand;
        [SerializeField] private SportHand _rightHand;

        [SerializeField] private float _handSpeedLimitToPunch = 0.3f;
        [SerializeField] private float _touchFactor = 4f;
        [SerializeField] private float _minPunchStrength = 0.5f;

        public Vector3 LastPunchedDir { get; private set; }
        public float LastPunchStrenght { get; private set; }

        [Space(3f)]
        [SerializeField] private BallOnline _ball;

        private bool _isBothHandsGrab;

        private bool _isBallAtCenterOfHands;

        private bool _isLeftHandGrabbed;
        private bool _isRightHandGrabbed;

        public Vector3 LeftHandControllerVelocity {get; private set;}
        public Vector3 RightHandControllerVelocity {get; private set;}

        private IEnumerator Start()
        {
            yield return StartCoroutine(SearchBall());

            _rightHand.OnGrabStopped += PunchBallOnGrabStopped;
            _leftHand.OnGrabStopped += PunchBallOnGrabStopped;
        }

        private IEnumerator SearchBall()
        {
            var delay = new WaitForSeconds(0.4f);
            while (_ball == null)
            {
                _ball = FindObjectOfType<BallOnline>();
                yield return delay;
            }
        }

        void Update()
        {

            //TODO: 
            // -[PROCESS] Realize both-hands ball trhrowing by button pressed
            // - COMPLETE: Ball grabbing with one hand
            // GRAB >
            /*
             * You can check the static float from
             * OVRInput.Get(Axis.1D.PrimaryHandTrigger, OVRInput.Controller.LTouch), 
             * 0 is not pressed, 1 is pressed to the end.
             */

            LeftHandControllerVelocity = CalculateHandVelocityByControllers(_leftHand);
            RightHandControllerVelocity = CalculateHandVelocityByControllers(_rightHand);

            _isBothHandsGrab = IsHandGrabBall(_leftHand) && IsHandGrabBall(_rightHand); 
            
            if (_isBothHandsGrab)
            {
                if(!_isBallAtCenterOfHands) {
                    _isBallAtCenterOfHands = true;
                }
                GrabBallAtCenterBetweenHands();               
            }
            if(!_isBallAtCenterOfHands)
            {
                if (IsHandGrabBall(_leftHand))
                {
                    if(_leftHand.IsGrabbedBall == false) 
                    {
                        _leftHand.IsGrabbedBall = true;
                    }
                    GrabBall(_leftHand);                    
                }

                if (IsHandGrabBall(_rightHand))
                {
                    if(_rightHand.IsGrabbedBall == false) 
                    {
                        _rightHand.IsGrabbedBall = true;
                    }
                    GrabBall(_rightHand); 
                }
            }           

            if(_isBallAtCenterOfHands) 
            {
                if(!_isLeftHandGrabbed || !_isRightHandGrabbed) 
                {
                    _isBallAtCenterOfHands = false;
                    PunhBallFromBothHands();                   
                }
            }

            if(_rightHand.IsGrabbedBall) 
            {
                if(!_isRightHandGrabbed) 
                {
                    _rightHand.IsGrabbedBall = false;
                }
            }

            if(_leftHand.IsGrabbedBall) 
            {
                if(!_isLeftHandGrabbed) 
                {
                    _leftHand.IsGrabbedBall = false;
                }
            }
         
        }

        private Vector3 CalculateHandVelocityByControllers(SportHand hand) 
        {
            var isLeft = hand.IsLeft;
            Vector3 controllerVelocity;
            var controller = isLeft ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
            controllerVelocity = OVRInput.GetLocalControllerVelocity(controller);

            return controllerVelocity;
        }

        private bool IsHandGrabBall(SportHand hand)
        {
            //0 is not pressed, 1 is pressed to the end.
            var controller = hand.IsLeft ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;

            var grabValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller);
            if(_isDebug)
            {
                if(hand.IsLeft) 
                {
                    grabValue = _isLeftHandGrabDebug ? 1f : 0f;
                }
                else
                {
                    grabValue = _isRightHandGrabDebug ? 1f : 0f;
                }
                
            }
            var isGrabbed = Mathf.Approximately(grabValue, 1f);
            if(hand.IsLeft) 
            {
                _isLeftHandGrabbed = isGrabbed;
            }
            else
            {
                _isRightHandGrabbed = isGrabbed;
            }

            return isGrabbed;
        }

        private Vector3 GetBallPositionOppositeHand(SportHand hand)
        {
            if (_ball == null)
                return Vector3.zero;
            var ballOffset = _ball.Collider.radius;
            var handPosition = hand.transform.position;
            var ballPosition = handPosition + (-hand.transform.up) * ballOffset;

            return ballPosition;
        }

        private void GrabBall(SportHand hand)
        {
            if (_ball == null)
                return;

            var ballPosition = GetBallPositionOppositeHand(hand);

            if (hand.IsCatchedBall)
            {
                SetBallPosition(ballPosition);
                SetBallVelocity(Vector3.zero);
            }
        }

        private void GrabBallAtCenterBetweenHands() 
        {
            var middlePosition = (_leftHand.transform.position + _rightHand.transform.position) / 2;
            SetBallPosition(middlePosition);
            SetBallVelocity(Vector3.zero);
        }

        private void PunhBallFromBothHands() 
        {
            if(_isDebug) 
            {
                Debug.Log("Ball punched from both hands!");
            }    

            var handsVelocityRaw = LeftHandControllerVelocity + RightHandControllerVelocity;
            var handsVelocityNormalized = handsVelocityRaw.normalized;
            var strength = _touchFactor * handsVelocityNormalized.magnitude; 
             _ball.AffectToBall(handsVelocityNormalized, strength);
             if (_isDebug)
            {
                Debug.Log($"BALL PUCHED BY CENTER, to: {handsVelocityNormalized}, strength: {strength}");
            }
        }

        private void PunchBallOnGrabStopped(SportHand hand)
        {
            var rightPos = hand.IsLeft ? -hand.transform.right : hand.transform.right;
            var punchPosition = ((-hand.transform.up) + (rightPos / 2f)).normalized;

            var handVelocityRaw = hand.IsLeft ? LeftHandControllerVelocity : RightHandControllerVelocity;
            var handVelocityNormalized = handVelocityRaw.normalized;
            var strength = _touchFactor * handVelocityNormalized.magnitude;

            if(Mathf.Approximately(strength, 0f) && _isDebug)
            {
                strength = _minPunchStrength * _touchFactor;
            }

            LastPunchStrenght = strength;
            LastPunchedDir = punchPosition;

            _ball.AffectToBall(handVelocityNormalized, strength);

            if (_isDebug)
            {
                Debug.Log($"BALL PUCHED BY {hand.gameObject.name}, to: {handVelocityNormalized}");
            }
        }

        private bool IsHandPunchBall(SportHand hand)
        {
            if (_ball != null && hand.IsCatchedBall && hand.HandSpeed >= _handSpeedLimitToPunch)
            {
                var strength = hand.HandSpeed * _touchFactor;
                _ball.AffectToBallAtPosition(hand.LastHittedPosition, strength);
                return true;
            }

            return false;
        }

        private void SetBallPosition(Vector3 position)
        {
            if(_ball != null)
            {
                _ball.SetPosition(position);
            }
        }

        private void SetBallVelocity(Vector3 velocity)
        {
            if(_ball != null)
            {
                _ball.SetVelocity(velocity);
            }
        }


    }
}
