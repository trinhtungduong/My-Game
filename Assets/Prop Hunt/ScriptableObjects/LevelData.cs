using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Data", fileName = "New Level Data")]
public class LevelData : ScriptableObject
{
    public RoomBase roomBase;
    public List<RoomNormal> listRoomNormals;
    public RoomEnd roomEnd;
    public int maxRooms;

    public RoomNormal RandomNormalRoom()
    {
        return listRoomNormals[0];
    }
}
