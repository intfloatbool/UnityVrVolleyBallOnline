using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LienarTransformView : MonoBehaviourPun, IPunObservable
{

    [SerializeField] private float _smoothPositionSpeed = 6f;
    [SerializeField] private float _smoothRotationSpeed = 6f;

    private Vector3 _currentPosition;
    private Quaternion _currentRotation;



    private void Update()
    {
        if(photonView.IsMine)
        {
            _currentPosition = transform.position;
            _currentRotation = transform.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _currentPosition, _smoothPositionSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _currentRotation, _smoothRotationSpeed * Time.deltaTime);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(_currentPosition);
            stream.SendNext(_currentRotation);
        }
        else
        {
            _currentPosition = (Vector3) stream.ReceiveNext();
            _currentRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
