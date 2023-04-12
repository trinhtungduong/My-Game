using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        var operation = SceneManager.LoadSceneAsync(1);
    }
}
