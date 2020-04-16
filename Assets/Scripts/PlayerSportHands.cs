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

        [Space(3f)]
        [SerializeField] private BallOnline _ball;

        private bool isCanGrab;

        private IEnumerator Start()
        {
            yield return StartCoroutine(SearchBall());
        }

        private IEnumerator SearchBall()
        {
            var delay = new WaitForSeconds(0.4f);
            while(_ball == null)
            {
                _ball = FindObjectOfType<BallOnline>();
                yield return delay;
            }
        }

        void Update()
        {

            //TODO: 
            // -Realize both-hands ball trhrowing by button pressed
            // -Ball grabbing with one hand
            // GRAB >
            /*
             * You can check the static float from
             * OVRInput.Get(Axis.1D.PrimaryHandTrigger, OVRInput.Controller.LTouch), 
             * 0 is not pressed, 1 is pressed to the end.
             */

            if (IsHandGrabBall(_leftHand))
                return;
            if (IsHandGrabBall(_rightHand))
                return;

            if (IsHandPunchBall(_leftHand))
                return;

            if (IsHandPunchBall(_rightHand))
                return;


            isCanGrab = _leftHand.IsCatchedBall && _rightHand.IsCatchedBall;

            if (isCanGrab)
            {
                var middlePosition = (_leftHand.transform.position + _rightHand.transform.position) / 2;
                SetBallPosition(middlePosition);
                SetBallVelocity(Vector3.zero);
            }        
        }

        private bool IsHandGrabBall(SportHand hand)
        {
            //0 is not pressed, 1 is pressed to the end.
            var controller = hand.IsLeft ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;

            var grabValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller);
            var isGrabbed = Mathf.Approximately(grabValue, 1f);

            if(_isDebug)
            {
                var key = hand.IsLeft ? _leftGrabDebugKey : _rightGrabDebugKey;
                isGrabbed = Mathf.Approximately(grabValue, 1f) || Input.GetKey(key); 
            }

            if (_ball != null && hand.IsCatchedBall && isGrabbed)
            {
                var ballOffset = _ball.Collider.radius;
                var handPosition = hand.transform.position;
                var ballPosition = handPosition + (-hand.transform.up) * ballOffset;
                SetBallPosition(ballPosition);
                SetBallVelocity(Vector3.zero);

                if(_isDebug)
                {
                    Debug.Log($"Hand: {hand.gameObject.name} grabbed ball!");
                }
            }
            return false;
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
