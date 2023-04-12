using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManagerOnlineScene : MonoBehaviour
{
    public static MenuManagerOnlineScene Instance;
    public Menu[] menus;
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
}
