using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleMapManager : MonoBehaviour
{
    public static SingleMapManager Instance;
    public List<LevelData> listLevelDatas;
    public LevelData levelData;
    public int numOfRooms;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void Start()
    {
        numOfRooms = 0;
        levelData = listLevelDatas[0];
        InstatiateBaseRoom();
    }

    public void InstatiateBaseRoom()
    {
        RoomBase roomBase = Instantiate(levelData.roomBase, Vector3.zero, Quaternion.identity);
        numOfRooms += 1;
        roomBase.SetupRoom();
    }
    public RoomInMap RandomNextRoom()
    {
        numOfRooms += 1;
        if (numOfRooms < levelData.maxRooms)
            return levelData.RandomNormalRoom();
        else if (numOfRooms == levelData.maxRooms)
            return levelData.roomEnd;

        return null;
    }
}
