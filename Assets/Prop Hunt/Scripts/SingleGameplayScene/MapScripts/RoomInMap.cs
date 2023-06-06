using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInMap : MonoBehaviour
{
    [SerializeField] protected Transform nextRoomSpawnPos;
    [SerializeField] protected Door door;
    public virtual void InstatiateRoom(RoomInMap _nextRoom)
    {
        if (_nextRoom != null)
        {
            var nextRoom = Instantiate(_nextRoom, nextRoomSpawnPos);
            nextRoom.transform.localPosition = Vector3.zero;
            nextRoom.transform.localRotation = Quaternion.identity;
            nextRoom.SetupRoom();
            door.nextRoom = nextRoom;
        }
    }
    public virtual void SetupRoom()
    {

    }
}
