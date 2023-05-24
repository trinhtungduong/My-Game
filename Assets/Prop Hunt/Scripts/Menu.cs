using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    [HideInInspector]
    public bool isOpen;
    public void OpenMenu()
    {
        isOpen = true;
        gameObject?.SetActive(true);
    }
    public void CloseMenu()
    {
        isOpen = false;
        gameObject?.SetActive(false);
    }
}
