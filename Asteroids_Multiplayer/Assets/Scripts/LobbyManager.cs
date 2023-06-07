using System;
using System.Collections;
using System.Collections.Generic;
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
        networkManager.OnServerStopped += LeaveServer;
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
        networkManager.OnServerStopped -= LeaveServer;
    }

    /// <summary>
    /// Being called on client and server
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientConnect(ulong _clientId)
    {
        Debug.Log("Client connected to the lobby");

        if (IsHost)
            AddPlayerToLobbyServerRpc(_clientId);
    }

    private void OnClientDisconnected(ulong _clientId)
    {
        RemovePlayerToLobbyClientRpc(_clientId);
    }

    private void LeaveServer(bool obj)
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    [ServerRpc]
    private void AddPlayerToLobbyServerRpc(ulong _clientId)
    {
        GameObject spawnedPrefab = Instantiate(lobbySlotPrefab);

        spawnedPrefab.GetComponent<NetworkObject>().Spawn();
        // Has to be spawned first over the network
        spawnedPrefab.transform.SetParent(prefabTarget);

        AddPlayerToLobbyClientRpc(_clientId, spawnedPrefab.GetComponent<NetworkObject>().NetworkObjectId);
    }

    [ClientRpc]
    private void AddPlayerToLobbyClientRpc(ulong _clientId, ulong _networkObjectId)
    {
        NetworkObject networkObject = GetNetworkObject(_networkObjectId);
        GameObject spawnedPrefab = networkObject.gameObject;

        LobbySlot lobbySlot = spawnedPrefab.GetComponent<LobbySlot>();

        if (IsHost)
            PlayerDataManager.instance.AddPlayerData(_clientId, lobbySlot);
    }

    [ClientRpc]
    private void RemovePlayerToLobbyClientRpc(ulong _clientId)
    {
        //PlayerDataManager.instance.RemovePlayerData(_clientId);
    }

    public void StartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void ReadyButton()
    {
    }
}