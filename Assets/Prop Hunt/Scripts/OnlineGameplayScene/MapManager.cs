using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public GameState gameState;
    public List<PlayerController> listPlayerInRoom;
    public List<Transform> listSpawns;

    public bool gameStart;
    public DragonControllerTest boss;

    public TMP_Text resultText;
    public GameObject endGamePanel;
    public Image playerHealth;

    private void Awake()
    {
        instance = this;      
    }
    private void Update()
    {
        if(!gameStart && PhotonNetwork.IsMasterClient)
        {
            if(listPlayerInRoom.Count == PhotonNetwork.PlayerList.Length)
            {
                gameStart = true;
                StartCoroutine(StartBoss());
            }
        }
    }
    IEnumerator StartBoss()
    {
        yield return new WaitForSeconds(3f);
        gameState = GameState.Playing;
        boss.StartBoss();
    }
    public void AddNewPlayer(PlayerController newPlayer)
    {
        Player[] players = PhotonNetwork.PlayerList;
        int indexSpawn = 0;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == newPlayer.photonView.Owner)
            {
                indexSpawn = i;
                break;
            }

        }

        if (indexSpawn >= listPlayerInRoom.Count)
            listPlayerInRoom.Add(newPlayer);
        else
            listPlayerInRoom.Insert(indexSpawn, newPlayer);
    }
    public void UpdateAlivePlayer()
    {
        int count = 0;
        for (int i = 0; i < listPlayerInRoom.Count; i++)
        {
            if (listPlayerInRoom[i] != null)
            {
                if (listPlayerInRoom[i].isAlive)
                    count += 1;
            }
        }

        if(count == 0)
        {
            EndGameLose();
        }
    }
    public void UpdatePlayerHealth(float curHealth, float maxHealth)
    {
        playerHealth.fillAmount = curHealth / maxHealth;
    }
    public void InActivePlayer(PlayerController player)
    {
        for (int i = 0; i < listPlayerInRoom.Count; i++)
        {
            if (listPlayerInRoom[i] == player)
            {
                listPlayerInRoom[i] = null;
            }
        }
    }
    public Transform GetSpawnPoint(int index)
    {
        return listSpawns[index];
    }
    public Transform GetRandomTarget()
    {
        int index = Random.Range(0, listPlayerInRoom.Count);
        for(int i = 0; i < listPlayerInRoom.Count; i++)
        {
            if(listPlayerInRoom[index] != null && listPlayerInRoom[i].isAlive)
            {
                return listPlayerInRoom[index].transform;
            }
        }
        return listPlayerInRoom[0].transform;
    }
    public void EndGameWin()
    {
        gameState = GameState.EndGame;
        Destroy(RoomManager.Instance.gameObject);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        resultText.text = "You Win";
        endGamePanel.SetActive(true);
        Invoke(nameof(EndGame), 5f);
    }
    public void EndGameLose()
    {
        gameState = GameState.EndGame;
        Destroy(RoomManager.Instance.gameObject);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        resultText.text = "You Lose";
        endGamePanel.SetActive(true);
        Invoke(nameof(EndGame), 5f);
    }
    public void EndGame()
    {
        for (int i = 0; i < listPlayerInRoom.Count; i++)
        {
            if (listPlayerInRoom[i] != null)
            {
                listPlayerInRoom[i].RemovePlayer();
            }
        }

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(2);
    }
}
public enum GameState
{
    Waiting,
    Playing,
    EndGame
}
