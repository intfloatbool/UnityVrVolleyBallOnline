using System;
using UnityEngine;

namespace VrVolleyball
{
    public class SportHand : MonoBehaviour
    {
        [SerializeField] private float _rayLength = 0.1f;
        [SerializeField] private float _rayRadius = 0.03f;
        [SerializeField] private float _overlappingRadius = 0.09f;
        [SerializeField] private float _speedCheckingDelay = 1f;

        [Space(2f)]
        [SerializeField] private bool _isDebug = true;
        
        [Space(5f)]
        [Header("Runtime references")]
        [SerializeField] private bool _isCatchedBall;
        public bool IsCatchedBall 
        {
            get { return _isCatchedBall; }
            set { this._isCatchedBall = value; }
        }

        [SerializeField] private BallOnline _currentBall;
        public BallOnline CurrentBall => _currentBall;

        [SerializeField] private Vector3 _lastDirection;
        public Vector3 LastDirection => _lastDirection;

        [SerializeField] private float _handSpeed;
        public float HandSpeed => _handSpeed;

        [SerializeField] private bool _isLeftHand;
        public bool IsLeft => _isLeftHand;

        public Vector3 LastHittedPosition { get; private set; }

        public event Action<SportHand, BallOnline, Vector3> OnBallTouched = (hand, ball, directionWorldSpace) => { };

        private Vector3 _lastHandPosition;
        private float _handSpeedTimer;
        [SerializeField] private Vector3[] _directions;

        private Collider[] _overlappedColliders = new Collider[3];

        int layerMask;

        private void Start()
        {
            layerMask = 1 << 8;
            if (_directions.Length == 0)
            {
                InitDefaultDirections();
            }
        }

        private void InitDefaultDirections()
        {
            _directions = new Vector3[]
            {
                Vector3.left,
                Vector3.up,
                Vector3.right,
                Vector3.down,
                Vector3.forward,
                Vector3.back,
                new Vector3(1, 1, 0),
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 1),
                new Vector3(1, 0, 1),
                -new Vector3(1, 1, 0),
                -new Vector3(1, 1, 1),
                -new Vector3(0, 1, 1),
                -new Vector3(1, 0, 1),
            };
        }

        private void FixedUpdate()
        {
            TryFindBallBySphereOverlap();
            TryCatchBallHitPosition();
            CalcualteSpeedLoop();
        }

        private void CalcualteSpeedLoop()
        {
            if(_handSpeedTimer >= _speedCheckingDelay)
            {
                _lastHandPosition = transform.position;
                _handSpeedTimer = 0;
            }

            _handSpeed = (transform.position - _lastHandPosition).sqrMagnitude;
            _handSpeedTimer += Time.deltaTime;
        }

        private void TryFindBallBySphereOverlap()
        {
             
            var overlappedColliders = Physics.OverlapSphereNonAlloc(
                transform.position,
                _overlappingRadius,
                _overlappedColliders,
                layerMask);

            if(overlappedColliders > 0)
            {
                for(int i = 0; i < _overlappedColliders.Length; i++)
                {
                    var col = _overlappedColliders[i];

                    if (col == null)
                    {
                        ResetHand();
                        continue;
                    }
                    
                    _currentBall = col.GetComponent<BallOnline>();
                    if(_currentBall != null)
                    {
                        _isCatchedBall = true;
                        _lastDirection = col.ClosestPoint(_currentBall.transform.position);
                        break;
                    }
                    else 
                    {
                        ResetHand();
                    }
                }
            }
            else
            {
                ResetHand();
            }
        }


        private void TryCatchBallHitPosition()
        {
            for (int i = 0; i < _directions.Length; i++)
            {
                var direction = _directions[i];

                if (CatchBallBySyde(direction))
                {

                    _lastDirection = direction;
                    OnBallTouched(this, _currentBall, transform.TransformDirection(_lastDirection));
                    return;
                }
            }
        }

        private void TryCatchBallEachSideLoop()
        {      
            for (int i = 0; i < _directions.Length; i++)
            {
                var direction = _directions[i];
                
                if(CatchBallBySyde(direction))
                {
                    
                    _isCatchedBall = true;
                    _lastDirection = direction;
                    OnBallTouched(this, _currentBall, transform.TransformDirection(_lastDirection));
                    return;
                }              
            }         
            ResetHand();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _overlappingRadius);

            //Directions
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, LastHittedPosition);
        }

        private void ResetHand()
        {
            _isCatchedBall = false;
            _currentBall = null;
            _lastDirection = Vector3.zero;
        }

        private bool CatchBallHitPositionBySide(Vector3 rawDirection)
        {
            if (_isDebug)
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(rawDirection) * _rayLength, Color.red, 0.3f);
            }


            RaycastHit hit;
            if (Physics.SphereCast(transform.position,
                _rayRadius,
                transform.TransformDirection(rawDirection),
                out hit,
                _rayLength, layerMask)
                )
            {
                if (_isDebug)
                {
                    Debug.Log("Ball catched by side: " + rawDirection);
                }

                LastHittedPosition = hit.point;
                return true;
            }
            return false;
        }

        private bool CatchBallBySyde(Vector3 rawDirection)
        {
            if (_isDebug)
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(rawDirection) * _rayLength, Color.red, 0.3f);
            }


            RaycastHit hit;
            if(Physics.SphereCast(transform.position,
                _rayRadius, 
                transform.TransformDirection(rawDirection), 
                out hit, 
                _rayLength, layerMask)
                )
            {
                var ball = hit.collider.GetComponent<BallOnline>();
                if(ball != null)
                {
                    if(_isDebug)
                    {
                        Debug.Log("Ball catched by side: " + rawDirection);
                    }

                    LastHittedPosition = hit.point;
                    _currentBall = ball;
                    return true;
                                        
                }
            }
            return false;
        }
    }
}


