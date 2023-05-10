using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector]
    public PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    void Start()
    {
        Debug.Log(photonView.IsMine);
        if (photonView.IsMine)
        {
            CreateController();
        }
    }

    public void CreateController()
    {
        Debug.Log("Create Player Controller");
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.zero, Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", "Test"), Vector3.zero + Vector3.forward * 5f, Quaternion.identity);
    }
}
