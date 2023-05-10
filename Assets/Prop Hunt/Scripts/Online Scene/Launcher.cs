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
    public TMP_Text roomErrorMessage;
    public Transform roomListContent;
    public RoomListItem roomListItemPrefab;
    private Dictionary<RoomInfo, RoomListItem> cachedRoomList = new Dictionary<RoomInfo, RoomListItem>();
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
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 10
        };
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
        roomErrorMessage.text = "Create Room Failed\n" + message;
    }
    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
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
        Debug.Log("Update");
        for(int i = 0; i < roomList.Count; i++)
        {          
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info))
                {
                    Destroy(cachedRoomList[info].gameObject);
                    cachedRoomList.Remove(info);
                }
            }
            else
            {
                if (!cachedRoomList.ContainsKey(info))
                {
                    var newRoom = Instantiate(roomListItemPrefab, roomListContent);
                    newRoom.Setup(roomList[i]);
                    cachedRoomList[info] = newRoom;
                }
            }
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).SetUp(newPlayer);
    }
}
