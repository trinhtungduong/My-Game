using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManagerOnlineScene : MonoBehaviour
{
    public static MenuManagerOnlineScene Instance;
    public Menu[] menus;
    public List<Sprite> listMaps;
    public GameObject roomOverviewHolder;
    public Image roomOverview;
    [HideInInspector]
    public RoomListItem roomChoosed;
    public GameObject joinRoomButton;
    public GameObject errorMessage;
    private void Awake()
    {
        Instance = this;
    }
    public void OpenMenu(string menuName)
    {
        for(int i = 0; i < menus.Length; i++)
        {
            if(menus[i].menuName == menuName)
            {
                menus[i].OpenMenu();
            }
            else if (menus[i].isOpen)
            {
                CloseMenu(menus[i]);
            }
        }
    }
    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].isOpen)
            {
                CloseMenu(menus[i]);
            }
        }
        menu.OpenMenu();
    }
    public void CloseMenu(Menu menu)
    {
        menu.CloseMenu();
    }
    public void InitFindRoomPanel()
    {
        errorMessage.SetActive(false);
    }
    public void ResetFindRoomPanel()
    {
        errorMessage.SetActive(false);
        joinRoomButton.SetActive(false);
        roomOverviewHolder.SetActive(false);
    }
    public void ChooseRoomToJoin(RoomListItem room)
    {
        roomChoosed = room;
        joinRoomButton.SetActive(true);
        roomOverviewHolder.SetActive(true);
        roomOverview.sprite = listMaps[(int)(room.info.CustomProperties["indexMap"])];
    }
    public void JoinRoom()
    {
        if (roomChoosed != null)
        {
            if (roomChoosed.info.PlayerCount < roomChoosed.info.MaxPlayers)
                Launcher.Instance.JoinRoom(roomChoosed.info);
            else
                errorMessage.SetActive(true);
        }
    }
}
