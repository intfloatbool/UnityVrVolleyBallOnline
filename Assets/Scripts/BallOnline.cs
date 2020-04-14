using Passer;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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
                SetKinematic(true);
            }
        }

        public void SetKinematic(bool isKinematic)
        {
            _rb.isKinematic = isKinematic;
        }

        public void AffectToBall(Vector3 affectVector, float strength)
        {
            _rb.AddForce(affectVector * strength);
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
                transform.position = myHum.PlayerBody.position + (Vector3.forward * 0.3f) + (Vector3.up * 0.3f);
            }

            Debug.Log("BALL TO ME DEBUG!");
        }
    }
}

