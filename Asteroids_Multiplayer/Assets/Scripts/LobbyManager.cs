using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private GameObject lobbySlotPrefab;
    [SerializeField] private Transform prefabTarget;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startGameButton;

    private NetworkManager networkManager;

    private void Awake()
    {
        networkManager = NetworkManager.Singleton;
    }

    private void OnEnable()
    {
        networkManager.OnClientConnectedCallback += OnClientConnect;
        networkManager.OnClientDisconnectCallback += OnClientDisconnected;
    }


    private void Start()
    {
        if (IsHost)
            AddPlayerToLobbyServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    private void OnDisable()
    {
        networkManager.OnClientConnectedCallback -= OnClientConnect;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    /// <summary>
    /// Being called on client and server
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientConnect(ulong _clientId)
    {
        Debug.Log("Client connected to the lobby");

        // Perform any additional actions when a client joins the lobby
        AddPlayerToLobbyServerRpc(_clientId);
    }

    private void OnClientDisconnected(ulong _clientId)
    {
        RemovePlayerToLobbyServerRpc(_clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPlayerToLobbyServerRpc(ulong _clientId)
    {
        GameObject spawnedPrefab = Instantiate(lobbySlotPrefab, prefabTarget);
        spawnedPrefab.AddComponent<PlayerData>().SetPlayerData(_clientId);
        PlayerDataManager.AddPlayerData(spawnedPrefab.GetComponent<PlayerData>());
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemovePlayerToLobbyServerRpc(ulong _clientId)
    {
        PlayerDataManager.RemovePlayerData(_clientId);
    }

    public void StartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void ReadyButton()
    {
    }
}