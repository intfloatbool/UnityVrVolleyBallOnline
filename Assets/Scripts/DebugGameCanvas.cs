﻿using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VrVolleyball;

public class DebugGameCanvas : MonoBehaviour
{
    [SerializeField] private Text _leftHandSpeedText;
    [SerializeField] private Text _rightHandSpeedText;
    [SerializeField] private Text _ballVelocityText;
    [SerializeField] private Text _lastPunchPowerText;
    [SerializeField] private Text _lastPunchDirText;
    [SerializeField] private SportHand _leftHand;
    [SerializeField] private SportHand _rightHand;
    [SerializeField] private PlayerSportHands _sportHands;

    private BallOnline _ball;

    private readonly string ZERO_STR = "0.0";
    private void FixedUpdate()
    {
        ShowDebugInfo();
    }

    private void ShowDebugInfo()
    {

        if(_ball == null)
        {
            _ball = FindObjectOfType<BallOnline>();
        }

        if(_rightHandSpeedText != null && _rightHand != null)
        {
            _rightHandSpeedText.text = GetRounded(_rightHand.HandSpeed);
        }

        if (_leftHandSpeedText != null && _leftHand != null)
        {
            _leftHandSpeedText.text = GetRounded(_leftHand.HandSpeed);
        }

        if(_ball != null && _ballVelocityText != null)
        {
            _ballVelocityText.text = _ball.Rb.velocity.ToString();
        }

        if(_sportHands != null && _lastPunchPowerText != null)
        {
            _lastPunchPowerText.text = GetRounded(_sportHands.LastPunchStrenght);
        }

        if(_sportHands != null && _lastPunchDirText != null)
        {
            _lastPunchDirText.text = _sportHands.LastPunchedDir.ToString();
        }

    }

    private string GetRounded(float val)
    {
        if (Mathf.Approximately(val, 0))
            return ZERO_STR;
        return System.Math.Round((decimal)val, 4).ToString();
    }
}
