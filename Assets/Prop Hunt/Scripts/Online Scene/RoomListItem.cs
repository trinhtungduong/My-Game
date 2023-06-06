using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    public TMP_Text text;
    public TMP_Text numberOfPlayer;
    public Image glow;
    public RoomInfo info;
    private int numOfPlayers;

    public void Setup(RoomInfo _info)
    {
        text.text = _info.Name;
        numOfPlayers = _info.PlayerCount;
        numberOfPlayer.text = numOfPlayers.ToString() + "/" + _info.MaxPlayers.ToString();
        glow.color = (numOfPlayers == _info.MaxPlayers) ? Color.red : Color.green;
        info = _info;
    }
    public void OnClick()
    {
        //Launcher.Instance.JoinRoom(info);
        MenuManagerOnlineScene.Instance.ChooseRoomToJoin(this);
    }
}
