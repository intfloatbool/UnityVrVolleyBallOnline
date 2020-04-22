using System;
using UnityEngine;

namespace VrVolleyball
{
    public class SportHand : MonoBehaviour
    {
        [SerializeField] private float _overlappingRadius = 0.09f;
        
        [Space(5f)]
        [Header("Runtime references")]
        [SerializeField] private bool _isCatchedBall;

        [SerializeField] private bool _isGrabbedBall;
        public bool IsGrabbedBall 
        {
            get {return _isGrabbedBall;}
            set 
            {
                this._isGrabbedBall = value;

                if(this.IsGrabbedBall == true) 
                {
                    OnGrabbed(this);
                }
                else 
                {
                    OnGrabStopped(this);
                }
            }
        }
        public bool IsCatchedBall 
        {
            get { return _isCatchedBall; }
            set { this._isCatchedBall = value; }
        }

        public event Action<SportHand> OnGrabbed = (hand) => { Debug.Log($"{hand.name} grabbed!"); };
        public event Action<SportHand> OnGrabStopped = (hand) => { Debug.Log($"{hand.name} grab stopped!"); };

        [SerializeField] private BallOnline _currentBall;
        public BallOnline CurrentBall => _currentBall;

        [SerializeField] private bool _isLeftHand;
        public bool IsLeft => _isLeftHand;
        private Collider[] _overlappedColliders = new Collider[3];

        int layerMask;

        private void Start()
        {
            layerMask = 1 << 8;
        }

        private void FixedUpdate()
        {
            TryFindBallBySphereOverlap();
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
                    if(!col.tag.Equals(BallOnline.BallTag))
                    {
                        ResetHand();
                        continue;
                    }

                    _currentBall = col.GetComponent<BallOnline>();
                    
                    if(_currentBall != null)
                    {
                        _isCatchedBall = true;
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _overlappingRadius);
        }

        private void ResetHand()
        {
            _isCatchedBall = false;
            _currentBall = null;
        }
    }
}


