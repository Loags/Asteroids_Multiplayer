using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkManager : MonoBehaviour
{
    public static GameNetworkManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void JoinGame()
    {
        NetworkManager.Singleton.StartClient();
    }
}