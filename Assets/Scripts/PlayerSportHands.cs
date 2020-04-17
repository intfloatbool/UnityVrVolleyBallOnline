using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

namespace VrVolleyball
{
    public class PlayerSportHands : MonoBehaviour
    {
        [SerializeField] private KeyCode _leftGrabDebugKey = KeyCode.O;
        [SerializeField] private KeyCode _rightGrabDebugKey = KeyCode.P;

        [SerializeField] private bool _isDebug = true;
        [Space(3f)]
        [SerializeField] private SportHand _leftHand;
        [SerializeField] private SportHand _rightHand;

        [SerializeField] private float _handSpeedLimitToPunch = 0.3f;
        [SerializeField] private float _touchFactor = 4f;
        [SerializeField] private float _minTouchDetect = 0.1f;
        [SerializeField] private float _minPunchStrength = 0.5f;

        public Vector3 LastPunchedDir { get; private set; }
        public float LastPunchStrenght { get; private set; }

        [Space(3f)]
        [SerializeField] private BallOnline _ball;

        private bool _isCanGrab;

        private bool _isLeftHandGrabbed;
        private bool _isRightHandGrabbed;

        private IEnumerator Start()
        {
            yield return StartCoroutine(SearchBall());
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


            _isCanGrab = _leftHand.IsCatchedBall && _rightHand.IsCatchedBall;

            if (_isCanGrab)
            {
                var middlePosition = (_leftHand.transform.position + _rightHand.transform.position) / 2;
                SetBallPosition(middlePosition);
                SetBallVelocity(Vector3.zero);
            }
            else
            {
                if (IsHandGrabBall(_leftHand))
                {
                    GrabBall(_leftHand);                    
                }
                else
                {
                    PunchBallOnGrabStopped(_leftHand);
                }

                if (IsHandGrabBall(_rightHand))
                {
                    GrabBall(_rightHand); 
                }
                else
                {
                    PunchBallOnGrabStopped(_rightHand);
                }

            }
        }

        private bool IsHandGrabBall(SportHand hand)
        {
            //0 is not pressed, 1 is pressed to the end.
            var controller = hand.IsLeft ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;

            var grabValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller);
            var isGrabbed = Mathf.Approximately(grabValue, 1f);

            if (_isDebug)
            {
                var key = hand.IsLeft ? _leftGrabDebugKey : _rightGrabDebugKey;
                isGrabbed = Mathf.Approximately(grabValue, 1f) || Input.GetKey(key);
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

                if (_isDebug)
                {
                    Debug.Log($"Hand: {hand.gameObject.name} grabbed ball!");
                }

                if (hand.IsLeft)
                {
                    _isLeftHandGrabbed = true;
                }
                else
                {
                    _isRightHandGrabbed = true;
                }
            }
        }

        private void PunchBallOnGrabStopped(SportHand hand)
        {
            var isHandNotCatched = !hand.IsCatchedBall;
            var isHandWasGrabbed = hand.IsLeft ? _isLeftHandGrabbed : _isRightHandGrabbed;
            var rightPos = hand.IsLeft ? -hand.transform.right : hand.transform.right;
            var punchPosition = ((-hand.transform.up) + (rightPos / 2f)).normalized;
            if (isHandWasGrabbed && isHandNotCatched)
            {
                var strength = hand.HandSpeed * _touchFactor;
                if(hand.HandSpeed <= _minTouchDetect)
                {
                    strength = _minPunchStrength * _touchFactor;
                }
                LastPunchStrenght = strength;
                LastPunchedDir = punchPosition;
                _ball.AffectToBall(punchPosition, strength);

                if (_isDebug)
                {
                    Debug.Log($"BALL PUCHED BY {hand.gameObject.name}, to: {punchPosition}");
                }

                if(hand.IsLeft)
                {
                    _isLeftHandGrabbed = false;
                }
                else
                {
                    _isRightHandGrabbed = false;
                }
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
