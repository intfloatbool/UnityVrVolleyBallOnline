using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VrVolleyball
{
    public class BallOnline : MonoBehaviourPun
    {
        [SerializeField] private Rigidbody _rb;


        private void Awake()
        {
            if(photonView.IsMine == false)
            {
                _rb.isKinematic = true;
            }
        }

        public void AffectToBall(Vector3 affectVector, float strength)
        {
            _rb.AddForce(affectVector * strength);
        }
    }
}

