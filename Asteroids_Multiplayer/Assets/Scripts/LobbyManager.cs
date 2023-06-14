using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager instance;
    public List<LobbySlot> lobbySlots = new();

    [SerializeField] private GameObject lobbySlotPrefab;
    [SerializeField] private Transform prefabTarget;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startGameButton;

    private NetworkManager networkManager;

    private void Awake()
    {
        Debug.Log("LobbyManager awake");
        if (instance == null)
            instance = this;
        networkManager = NetworkManager.Singleton;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsHost) return;

        foreach (var playerData in PlayerDataManager.instance.playerDatas)
        {
            AddLobbySlotServerRpc(playerData.ID);
            UpdateSlots();
            Debug.Log("Create LobbySlots as host on Awake");
        }
    }

    private void OnEnable()
    {
        networkManager.OnClientConnectedCallback += OnClientConnect;
        networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        networkManager.OnServerStopped += LeaveServer;
    }

    private void Start()
    {
        //if (IsHost)
        //AddPlayerToLobbyServerRpc(NetworkManager.Singleton.LocalClientId);

        if (!IsHost)
            startGameButton.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        networkManager.OnClientConnectedCallback -= OnClientConnect;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        networkManager.OnServerStopped -= LeaveServer;
    }

    /// <summary>
    /// Being called on client and host
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientConnect(ulong _clientId)
    {
        Debug.Log("Client connected to the lobby");
        PlayerDataManager.instance.AddNewPlayerDataServerRpc(networkManager.LocalClientId, false);

        //if (IsHost)
        //AddPlayerToLobbyServerRpc(_clientId);
    }

    private void OnClientDisconnected(ulong _clientId)
    {
        Debug.Log("Client disconnected from the lobby");

        //RemovePlayerToLobbyClientRpc(_clientId);
    }

    private void LeaveServer(bool obj)
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    [ServerRpc]
    public void AddLobbySlotServerRpc(ulong _clientId)
    {
        GameObject spawnedPrefab = Instantiate(lobbySlotPrefab);

        spawnedPrefab.GetComponent<NetworkObject>().Spawn();
        // Has to be spawned over the network first before re-parenting it
        spawnedPrefab.transform.SetParent(prefabTarget);

        AddLobbySlotClientRpc();
    }

    [ClientRpc]
    private void AddLobbySlotClientRpc()
    {
        lobbySlots = FindObjectsOfType<LobbySlot>().ToList();
    }


    public void UpdateSlots()
    {
        // Used FindObjectOfType to get all lobbyslots (reversed order)
        int reverseIndex = lobbySlots.Count - 1;
        Debug.Log("Slots Amount: " + lobbySlots.Count + "     PlayerDataAmount: " +
                  PlayerDataManager.instance.playerDatas.Count);
        foreach (var lobbySlot in lobbySlots)
        {
            PlayerDataManager.PlayerData currentPlayerData = PlayerDataManager.instance.playerDatas[reverseIndex];
            lobbySlot.UpdateSlotWithPlayerData(currentPlayerData.ID, currentPlayerData.IsReady);
            reverseIndex -= 1;
        }
    }

    /*[ClientRpc]
    private void RemovePlayerToLobbyClientRpc(ulong _clientId)
    {
        PlayerDataManager.instance.RemovePlayerData(_clientId);
    }*/


    /// <summary>
    /// Can only be called from Host
    /// </summary>
    public void StartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    /*public void ReadyButton()
    {
        ToggleReadyServerRpc(networkManager.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleReadyServerRpc(ulong _clientId)
    {
        ToggleReadyClientRpc(_clientId);
    }

    [ClientRpc]
    private void ToggleReadyClientRpc(ulong _clientId)
    {
        if (IsHost)
        {
            PlayerDataManager.instance.isPlayerReady[(int)_clientId] =
                !PlayerDataManager.instance.isPlayerReady[(int)_clientId];
            ToggleStartButton();
        }

        foreach (var lobbySlot in PlayerDataManager.instance.lobbySlots)
            if (lobbySlot.playerId == _clientId)
                lobbySlot.isPlayerReady = PlayerDataManager.instance.isPlayerReady[(int)_clientId];
    }

    private void ToggleStartButton()
    {
        bool active = false;
        foreach (var isPlayerReady in PlayerDataManager.instance.isPlayerReady)
        {
            if (!isPlayerReady)
            {
                active = false;
                break;
            }

            active = true;
        }

        startGameButton.gameObject.SetActive(active);
    }*/
}