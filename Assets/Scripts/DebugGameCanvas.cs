using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VrVolleyball;

public class DebugGameCanvas : MonoBehaviour
{
    [SerializeField] private Text _leftHandSpeedText;
    [SerializeField] private Text _rightHandSpeedText;
    [SerializeField] private Text _ballVelocityText;
    [SerializeField] private SportHand _leftHand;
    [SerializeField] private SportHand _rightHand;
    [SerializeField] private PlayerSportHands _sportHands;

    private BallOnline _ball;


    private void FixedUpdate()
    {
        ShowDebugInfo();
    }

    private void ShowDebugInfo()
    {
        if(_rightHandSpeedText != null && _rightHand != null)
        {
            _rightHandSpeedText.text = _rightHand.HandSpeed.ToString("#.00");
            if(_ball == null)
            {
                _ball = _rightHand.CurrentBall;
            }
        }

        if (_leftHandSpeedText != null && _leftHand != null)
        {
            _leftHandSpeedText.text = _leftHand.HandSpeed.ToString("#.00");
            if (_ball == null)
            {
                _ball = _leftHand.CurrentBall;
            }
        }

        if(_ball != null && _ballVelocityText != null)
        {
            _ballVelocityText.text = _ball.Rb.velocity.ToString();
        }

    }
}
