
using System.IO;
using UnityEngine;

public class RoomBase : RoomInMap
{
    public Transform playerStart;

    public override void SetupRoom()
    {
        base.SetupRoom();
        InitPlayer();        
        InstatiateRoom(SingleMapManager.Instance.RandomNextRoom());
    }
    public void InitPlayer()
    {
        Instantiate(Resources.Load(Path.Combine("PhotonPrefabs", "SinglePlayerController")), playerStart.position, Quaternion.identity);
    }
}
