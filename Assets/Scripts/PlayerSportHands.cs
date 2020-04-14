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

        [Space(3f)]
        [SerializeField] private BallOnline _ball;

        private bool isCanGrab;

        private IEnumerator Start()
        {
            yield return StartCoroutine(SearchBall());
        }

        private IEnumerator SearchBall()
        {
            var delay = new WaitForSeconds(1f);
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


            isCanGrab = _leftHand.IsCatchedBall && _rightHand.IsCatchedBall;

            if (isCanGrab && _ball != null)
            {
                var middlePosition = (_leftHand.transform.position + _rightHand.transform.position) / 2;
                _ball.transform.position = middlePosition;
                SetBallKinematicLocally(true);
            }
            else
            {
                isCanGrab = false;
                SetBallKinematicLocally(false);
            }           

        }

        private void SetBallKinematicLocally(bool isKinematic)
        {
            if (_ball != null && _ball.photonView.IsMine)
                _ball.SetKinematic(isKinematic);
        }
    }
}
