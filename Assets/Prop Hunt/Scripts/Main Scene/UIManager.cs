using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        MenuManagerMainScene.Instance.OpenMenu("home");
    }
    public void OpenOnlineMode()
    {
        MenuManagerMainScene.Instance.OpenMenu("loading");
        PhotonNetwork.OfflineMode = false;
        SceneManager.LoadSceneAsync(2);
    }
    public void OpenOfflineMode()
    {
        MenuManagerMainScene.Instance.OpenMenu("loading");
        PhotonNetwork.OfflineMode = true;
        SceneManager.LoadSceneAsync(1);
    }
}
