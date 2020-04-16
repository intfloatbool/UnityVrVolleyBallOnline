using Passer;
using Photon.Pun;
using System.Linq;
using UnityEngine;


namespace VrVolleyball
{
    public class BallOnline : MonoBehaviourPun
    {
        [SerializeField] private SphereCollider _collider;
        public SphereCollider Collider => _collider;

        [SerializeField] private Rigidbody _rb;
        public Rigidbody Rb => _rb;


        [SerializeField] private bool _isCheckingForDownfall = true;
        [SerializeField] private float _downFallY = -0.3f;
        [SerializeField] private Vector3 _startPosition = new Vector3(0, 4, 0);


        [Space(5f)]
        [Header("Debug")]
        [SerializeField] private bool _isDebug = true;
        [SerializeField] private Vector3 _debugForce;
        public Vector3 DebugForce => _debugForce;
        [SerializeField] private float _debugStrength;
        public float DebugStrength => _debugStrength;


        private void Start()
        {
            if(photonView.IsMine == false)
            {
                _rb.isKinematic = true;
            }

            if(_collider == null)
            {
                _collider = GetComponent<SphereCollider>();
            }
        }

        public void AffectToBallAtPosition(Vector3 fromPosition, float strength)
        {
            Vector3 direction = _rb.transform.position - fromPosition;         
            if (photonView.IsMine)
            {
                _rb.AddForceAtPosition(direction.normalized * strength, fromPosition);
            }
            else
            {
                photonView.RPC("AffectToBallAtPositionRPC", RpcTarget.MasterClient, fromPosition, direction, strength);
            }
        }

        [PunRPC]
        public void AffectToBallAtPositionRPC(Vector3 fromPosition, Vector3 direction, float strength)
        {
            _rb.AddForceAtPosition(direction.normalized * strength, fromPosition);
        }

        public void AffectToBall(Vector3 affectVector, float strength)
        {
            if(photonView.IsMine)
            {
                _rb.AddForce(affectVector * strength);
            }
            else
            {
                photonView.RPC("AffectToBallRPC", RpcTarget.MasterClient, affectVector, strength);
            }
                    
        }

        [PunRPC]
        public void AffectToBallRPC(Vector3 affectVector, float strength)
        {
            _rb.AddForce(affectVector * strength);

            Debug.Log($"Affect from remote player! Vector: {affectVector}, strength: {strength}");
        }

        public void SetPosition(Vector3 position)
        {
            if(photonView.IsMine)
            {
                transform.position = position;
            }
            else
            {
                photonView.RPC("SetPositionRPC", RpcTarget.MasterClient, position);
            }            
        }

        [PunRPC]
        public void SetPositionRPC(Vector3 position)
        {
            transform.position = position;
        }

        public void SetVelocity(Vector3 velocity)
        {
            if(photonView.IsMine)
            {
                _rb.velocity = velocity;
            }
            else
            {
                photonView.RPC("SetVeloctyRPC", RpcTarget.MasterClient, velocity);
            }            
        }

        [PunRPC]
        public void SetVeloctyRPC(Vector3 velocity)
        {
            _rb.velocity = velocity;
        }

        private void Update()
        {
            if(_isDebug)
            {
                if(OVRInput.GetDown(OVRInput.RawButton.B) || Input.GetKeyDown(KeyCode.Z))
                {
                    BallToMeDebug();
                }
            }

            if(photonView.IsMine)
            {
                if (_isCheckingForDownfall && transform.position.y <= _downFallY)
                {
                    SetPosition(_startPosition);
                    SetVelocity(Vector3.zero);
                }
            }
        }

        [ContextMenu("Ball to me")]
        public void BallToMeDebug()
        {
            var humanoids = FindObjectsOfType<HumanoidControl>();
            var myHum = humanoids.FirstOrDefault(h => !h.isRemote);
            if(myHum != null)
            {
                var newPosition = myHum.PlayerBody.position +
                    (myHum.PlayerBody.forward * 0.3f) +
                    (myHum.PlayerBody.up * 0.3f);
                SetPosition(newPosition);
                SetVelocity(Vector3.zero);
            }

            Debug.Log("BALL TO ME DEBUG!");
        }
    }
}

