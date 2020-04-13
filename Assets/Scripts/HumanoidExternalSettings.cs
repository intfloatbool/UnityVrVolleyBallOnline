using Passer;
using System.Collections;
using UnityEngine;

namespace VrVolleyball
{
    public class HumanoidExternalSettings : MonoBehaviour
    {
        [SerializeField] private HumanoidControl _humanoid;

        [Space(5f)]
        [SerializeField] private bool _isTranslateByPhysics = true;

        [Space(5f)]
        [SerializeField] private bool _isAutoCalibrate = true;
        [SerializeField] private float _autoCalibrateDelay = 3f;
        [SerializeField] private int _autoCalibrateTimes = 6;

        private void Awake()
        {
            _humanoid.IsTranslateByPhysics = _isTranslateByPhysics;
        }

        private IEnumerator Start()
        {
            if(_isAutoCalibrate && !_humanoid.isRemote && _humanoid != null)
            {
                var waiting = new WaitForSeconds(_autoCalibrateDelay);
                var times = 0;
                while(times < _autoCalibrateTimes)
                {
                    yield return waiting;
                    _humanoid.Calibrate();
                    times++;
                }

                yield break;
            }
            else
            {
                yield break;
            }
        }

        private void Update()
        {
            if (_isAutoCalibrate && !_humanoid.isRemote && _humanoid != null)
            {
                if(OVRInput.GetDown(OVRInput.RawButton.A) || Input.GetKeyDown(KeyCode.C))
                {
                    _humanoid.Calibrate();
                }
            }
        }
    }

}