using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VrVolleyball
{
    public class GameOnlineManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private string _ballPrefabName = "ball";
        [SerializeField] private Vector3 _positionToInstantiate = new Vector3(0, 0.3f,0);

        public override void OnJoinedRoom()
        {
            if(PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate(_ballPrefabName, _positionToInstantiate, Quaternion.identity);
            }
        }
    }

}
