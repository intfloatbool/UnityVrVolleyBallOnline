using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Passer;
using System.Linq;

namespace VrVolleyball.SimpleAI
{
    public class AiPlayer : MonoBehaviour
    {
        private HumanoidControl _humanoidPlayerLocal;
        private BallOnline _ball;

        private bool _isInitialized;

        [SerializeField] private float _minZtoActive = 0.5f;
        private bool IsBallAtMySide => _ball != null && _ball.transform.position.z > _minZtoActive;
        
        [SerializeField] private float _minDistanceToPunch = 1f;

        private Vector3 _basicPos;

        private float Distance 
        {
            get {
                if(_ball == null)
                    return 10f;
                return Vector3.Distance(_ball.transform.position, transform.position);
            }
        }

        private bool IsReachedBall => Distance <= _minDistanceToPunch;

        [SerializeField] private float _punchStrength = 500f;
        [SerializeField] private float _punchYfactor = 0.5f;
        [SerializeField] private float _moveSpeed = 3f;

        [SerializeField] private Animator _animator;
        [SerializeField] private string _movingTriggerName;
        [SerializeField] private string _idleTriggerName;
        [SerializeField] private string _jumpTriggerName;

        private bool IsCanPunch => _punchTimer >= _punchDelay;
        [SerializeField] private float  _punchDelay = 1.5f;
        private float _punchTimer = 0f;

        IEnumerator Start()
        {
            _basicPos = transform.position;
            var waiting = new WaitForSeconds(1f);
            while(_humanoidPlayerLocal == null) 
            {
                var players = FindObjectsOfType<HumanoidControl>().ToList();
                _humanoidPlayerLocal = players.FirstOrDefault(hc => !hc.isRemote);
                if(_humanoidPlayerLocal != null) {
                    Debug.Log("AI Player has found local player!: " + _humanoidPlayerLocal.name);
                }
                yield return waiting;
            }

            while(_ball == null) {
                _ball = FindObjectOfType<BallOnline>();
                if(_ball != null) 
                {
                    Debug.Log("AI Player has found ball!: " + _ball.name);
                }
                yield return waiting;
            }

            _isInitialized = true;
        }

        void Update()
        {
            
           if(!_isInitialized)
                return;
            HandlePunchTimer();
            var ballPos = new Vector3(_ball.transform.position.x, transform.position.y, _ball.transform.position.z);
            
            if(IsReachedBall && IsCanPunch) {
                
                var relativePos = _humanoidPlayerLocal.transform.position - _ball.transform.position;
                var normalizedPos = relativePos.normalized;
                var distanceFromPlayer = Vector3.Distance(_ball.transform.position, _humanoidPlayerLocal.transform.position);
                var punchPos = new Vector3(
                    normalizedPos.x,
                    _punchYfactor,
                    normalizedPos.z
                );
                var strength = _punchStrength * distanceFromPlayer;
                _ball.AffectToBall(punchPos, strength);

                Debug.Log($"Punch to player! Strenght: {strength.ToString()}"); 
                Debug.Log($"PunchPos: {punchPos.ToString()}"); 

                if(_animator != null && !string.IsNullOrEmpty(_jumpTriggerName)) {
                    _animator.SetTrigger(_jumpTriggerName);
                }

                _punchTimer = 0f;
                return;
            }

            if(IsBallAtMySide) 
            {
                transform.LookAt(ballPos, Vector3.up); 
                MoveToPos(ballPos);
            } 
            else {

                if(transform.position.z > _basicPos.z) {
                    transform.LookAt(_basicPos, Vector3.up);
                    MoveToPos(_basicPos);
                }
                if(_animator != null && !string.IsNullOrEmpty(_idleTriggerName)) {
                    _animator.SetTrigger(_idleTriggerName);
                }
            }
        }

        private void HandlePunchTimer() 
        {
            _punchTimer += Time.deltaTime;
            if(_punchTimer >= float.MaxValue / 2f)
                _punchTimer = 0f;
        }

        private void MoveToPos(Vector3 pos) 
        {
            transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
            if(_animator != null && !string.IsNullOrEmpty(_movingTriggerName)) {
                _animator.SetTrigger(_movingTriggerName);
            }
        }

    }
}

