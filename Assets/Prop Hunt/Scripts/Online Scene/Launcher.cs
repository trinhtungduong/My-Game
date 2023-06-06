using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    [HideInInspector]
    public int indexMap;

    public TMP_Text numberOfPlayerInRoom;

    [Header("Start Room Setting")]
    public GameObject startGameButton;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            MenuManagerOnlineScene.Instance.OpenMenu("loading");
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            MenuManagerOnlineScene.Instance.OpenMenu("room");
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;

            roomNameText.text = PhotonNetwork.CurrentRoom.Name;

            numberOfPlayerInRoom.text = "Players: " +
                                         PhotonNetwork.CurrentRoom.PlayerCount.ToString() +
                                         "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();

            Player[] players = PhotonNetwork.PlayerList;

            foreach (Transform trans in playerListContent)
            {
                Destroy(trans.gameObject);
            }

            for (int i = 0; i < players.Length; i++)
            {
                Instantiate(playerListItemPrefab, playerListContent).SetUp(players[i]);
            }

            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }
    }
    public void Disconnect()
    {
        MenuManagerOnlineScene.Instance.OpenMenu("loading");
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnect from the server");
        if (RoomManager.Instance != null)
            Destroy(RoomManager.Instance.gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.SendRate = 40;
        PhotonNetwork.SerializationRate = 5;
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
            MaxPlayers = 2
        };
        Hashtable RoomCustomProps = new Hashtable();
        RoomCustomProps.Add("indexMap", indexMap);
        roomOptions.CustomRoomProperties = RoomCustomProps;
        roomOptions.CustomRoomPropertiesForLobby = new string[1] { "indexMap" };
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

        numberOfPlayerInRoom.text = "Players: " +
                                     PhotonNetwork.CurrentRoom.PlayerCount.ToString() +
                                     "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();

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
        roomErrorMessage.text = "Error: " + message;
    }
    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(3);
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
                    if (MenuManagerOnlineScene.Instance.roomChoosed == cachedRoomList[info])
                        MenuManagerOnlineScene.Instance.ResetFindRoomPanel();
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
                else
                {
                    cachedRoomList[info].Setup(roomList[i]);
                }
            }
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).SetUp(newPlayer);
        numberOfPlayerInRoom.text = "Players: " +
                                     PhotonNetwork.CurrentRoom.PlayerCount.ToString() +
                                     "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
    }
    public void SetRoomMessage(string content)
    {
        roomErrorMessage.text = content;
    }
}
