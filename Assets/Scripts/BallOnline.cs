using Passer;
using Photon.Pun;
using System.Linq;
using UnityEngine;


namespace VrVolleyball
{
    public class BallOnline : MonoBehaviourPun
    {
        [SerializeField] private Rigidbody _rb;

        [Space(5f)]
        [Header("Debug")]
        [SerializeField] private bool _isDebug = true;
        [SerializeField] private Vector3 _debugForce;
        public Vector3 DebugForce => _debugForce;
        [SerializeField] private float _debugStrength;
        public float DebugStrength => _debugStrength;

        private void Awake()
        {
            if(photonView.IsMine == false)
            {
                _rb.isKinematic = true;
            }
        }

        public void SetKinematic(bool isKinematic)
        {
            photonView.RPC("SetKinematicRPC", RpcTarget.All, isKinematic);
        }

        [PunRPC]
        public void SetKinematicRPC(bool isKinematic)
        {
            if(photonView.IsMine)
            {
                _rb.isKinematic = isKinematic;
            }
        }

        public void AffectToBall(Vector3 affectVector, float strength)
        {
            photonView.RPC("AffectToBallRPC", RpcTarget.All, affectVector, strength);
        }

        [PunRPC]
        public void AffectToBallRPC(Vector3 affectVector, float strength)
        {
            if(photonView.IsMine)
            {
                _rb.isKinematic = false;
                _rb.AddForce(affectVector * strength);
            }
        }

        public void SetPosition(Vector3 position)
        {
            photonView.RPC("SetPositionRPC", RpcTarget.All, position);
        }

        [PunRPC]
        public void SetPositionRPC(Vector3 position)
        {
            if(photonView.IsMine)
            {
                transform.position = position;
            }
        }

        public void SetVelocity(Vector3 velocity)
        {
            photonView.RPC("SetVeloctyRPC", RpcTarget.All, velocity);
        }

        [PunRPC]
        public void SetVeloctyRPC(Vector3 velocity)
        {
            if(photonView.IsMine)
            {
                _rb.velocity = velocity;
            }
        }

        private void Update()
        {
            if(_isDebug && photonView.IsMine)
            {
                if(OVRInput.GetDown(OVRInput.RawButton.B) || Input.GetKeyDown(KeyCode.Z))
                {
                    BallToMeDebug();
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

