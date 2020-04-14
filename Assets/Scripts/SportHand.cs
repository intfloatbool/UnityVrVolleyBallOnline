using UnityEngine;

namespace VrVolleyball
{
    public class SportHand : MonoBehaviour
    {
        [SerializeField] private float _rayLength = 0.1f;
        [SerializeField] private float _rayRadius = 0.03f;

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

        public Vector3 LastHittedPosition { get; private set; }

        private void Update()
        {
            TryCatchBallEachSideLoop();
        }

        private void TryCatchBallEachSideLoop()
        {
            var directions = new Vector3[]
            {
                Vector3.left,
                Vector3.up,
                Vector3.right,
                Vector3.down,
                Vector3.forward,
                Vector3.back
            };

            for(int i = 0; i < directions.Length; i++)
            {
                var direction = directions[i];
                if(CatchBallBySyde(direction))
                {
                    
                    _isCatchedBall = true;
                    _lastDirection = direction;
                    return;
                }              
            }

            _isCatchedBall = false;
            _currentBall = null;
            _lastDirection = Vector3.zero;
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
                _rayLength)
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


