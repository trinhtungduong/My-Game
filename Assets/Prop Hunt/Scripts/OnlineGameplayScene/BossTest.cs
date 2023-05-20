using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTest : MonoBehaviour
{
    public static BossTest instance;
    public GameState gameState;
    public List<PlayerController> listPlayerInRoom;
    public List<Transform> listSpawns;

    private void Awake()
    {
        instance = this;
    }

    public void AddNewPlayer(PlayerController newPlayer)
    {
        listPlayerInRoom.Add(newPlayer);
    }
    public void InActivePlayer(PlayerController player)
    {
        for(int i = 0; i < listPlayerInRoom.Count; i++)
        {
            if(listPlayerInRoom[i] == player)
            {
                listPlayerInRoom[i] = null;
            }
        }
    }
    public Transform GetSpawnPoint(int index)
    {
        return listSpawns[index];
    }
}

public enum GameState
{
    Playing,
    EndGame
}
