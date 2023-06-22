using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadHelper : MonoBehaviour
{
    private void Awake()
    {
        gameObject.AddToDontDestroyOnLoad();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (!NetworkManager.Singleton.IsHost) return;
        if (SceneManager.GetActiveScene().name == "Game" || SceneManager.GetActiveScene().name == "MainMenu")
            transform.GetChild(0).gameObject.SetActive(false);
        else if (SceneManager.GetActiveScene().name == "Lobby")
            transform.GetChild(0).gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}