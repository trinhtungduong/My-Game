using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [Header("Room and Player Setting")]
    public TMP_InputField roomNameInputField;
    public TMP_Text roomNameText;
    public Transform roomListContent;
    public RoomListItem roomListItemPrefab;
    public Transform playerListContent;
    public PlayerListItem playerListItemPrefab;

    [Header("Start Room Setting")]
    public GameObject startGameButton;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        MenuManagerOnlineScene.Instance.OpenMenu("loading");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnJoinedLobby()
    {      
        MenuManagerOnlineScene.Instance.OpenMenu("main home");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
    }
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text)) return;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
        MenuManagerOnlineScene.Instance.OpenMenu("loading");
    }
    public void JoinRoom(RoomInfo info)
    {       
        PhotonNetwork.JoinRoom(info.Name);
        MenuManagerOnlineScene.Instance.OpenMenu("loading");     
    }
    public override void OnJoinedRoom()
    {
        MenuManagerOnlineScene.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform trans in playerListContent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);      
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MenuManagerOnlineScene.Instance.OpenMenu("error panel");
    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(2);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManagerOnlineScene.Instance.OpenMenu("loading");
    }
    public override void OnLeftRoom()
    {
        MenuManagerOnlineScene.Instance.OpenMenu("main home");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for(int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).Setup(roomList[i]);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).SetUp(newPlayer);
    }
}
