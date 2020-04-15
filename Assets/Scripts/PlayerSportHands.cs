using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

namespace VrVolleyball
{
    public class PlayerSportHands : MonoBehaviour
    {
        [SerializeField] private bool _isDebugRightHandGrab;
        [SerializeField] private bool _isDebugLeftHandGrab;

        [SerializeField] private KeyCode _leftGrabKey = KeyCode.O;
        [SerializeField] private KeyCode _rightGrabKey = KeyCode.P;
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
            /*
             * You can check the static float from
             * OVRInput.Get(Axis.1D.PrimaryHandTrigger, OVRInput.Controller.LTouch), 
             * 0 is not pressed, 1 is pressed to the end.
             */

            if (IsHandPunchBall(_leftHand))
                return;

            if (IsHandPunchBall(_rightHand))
                return;


            //TODO: remove this logic ?
            return;
            isCanGrab = _leftHand.IsCatchedBall && _rightHand.IsCatchedBall;

            if (isCanGrab)
            {
                var middlePosition = (_leftHand.transform.position + _rightHand.transform.position) / 2;
                SetBallPosition(middlePosition);
                SetBallVelocity(Vector3.zero);

            }        
        }

        private bool IsHandPunchBall(SportHand hand)
        {
            if (hand.IsCatchedBall && hand.HandSpeed >= _handSpeedLimitToPunch)
            {
                var transformedDirection = hand.transform.TransformDirection(hand.LastDirection);
                var forcePower = hand.HandSpeed * _touchFactor;
                hand.CurrentBall.AffectToBall(transformedDirection, forcePower);
                Debug.Log($"Affect to ball!  Hand: {hand.gameObject.name}\n " 
                    + "Direction: " +
                    transformedDirection + 
                    "\nForcePower: " + forcePower);
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

        private void SetBallKinematic(bool isKinematic)
        {
            if(_ball != null)
            {
                _ball.SetKinematic(isKinematic);
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
