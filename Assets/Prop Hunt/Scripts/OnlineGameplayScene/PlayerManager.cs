using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector]
    public PhotonView photonView;

    GameObject controller;

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
        Player[] players = PhotonNetwork.PlayerList;
        int indexSpawn = 0;
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i] == photonView.Owner)
            {
                indexSpawn = i;
                break;
            }

        }
        Transform spawnPoint = BossTest.instance.GetSpawnPoint(indexSpawn);
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPoint.position, Quaternion.identity, 0, new object[] { photonView.ViewID });
    }

    public void RemovePlayer()
    {
        PhotonNetwork.Destroy(controller);
        PhotonNetwork.Destroy(gameObject);
    }
}
