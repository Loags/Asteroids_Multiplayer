using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager instance;
    public List<LobbySlot> lobbySlots = new();

    [SerializeField] private GameObject lobbySlotPrefab;
    [SerializeField] private Transform prefabTarget;
    [SerializeField] private Button startGameButton;

    private NetworkManager networkManager;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        networkManager = NetworkManager.Singleton;
    }

    public override void OnNetworkSpawn()
    {
        InvokeRepeating(nameof(UpdateSlots), 0f, 0.25f);

        if (!IsHost) return;
        StartCoroutine(DelaySpawnSlot());
    }

    private IEnumerator DelaySpawnSlot()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (var playerData in PlayerDataManager.instance.playerDatas)
            AddLobbySlotServerRpc();
    }

    private void OnEnable()
    {
        networkManager.OnClientConnectedCallback += OnClientConnect;
        networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        networkManager.OnServerStopped += LeaveServer;
    }

    private void Start()
    {
        if (!IsHost)
            startGameButton.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        networkManager.OnClientConnectedCallback -= OnClientConnect;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        networkManager.OnServerStopped -= LeaveServer;
    }

    private void OnClientConnect(ulong _clientId)
    {
        PlayerDataManager.instance.AddNewPlayerDataServerRpc(networkManager.LocalClientId, false);

        if (IsHost)
            ToggleStartButton();
    }

    private void OnClientDisconnected(ulong _clientId)
    {
        RemovePlayerFromLobbyClientRpc();
    }

    private void LeaveServer(bool obj)
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    [ServerRpc]
    public void AddLobbySlotServerRpc()
    {
        GameObject spawnedPrefab = Instantiate(lobbySlotPrefab);
        spawnedPrefab.GetComponent<NetworkObject>().Spawn();
        spawnedPrefab.transform.SetParent(prefabTarget);

        UpdateLobbySlotListClientRpc();
    }

    [ClientRpc]
    private void UpdateLobbySlotListClientRpc() => lobbySlots = FindObjectsOfType<LobbySlot>().ToList();

    private void UpdateSlots()
    {
        int reverseIndex = lobbySlots.Count - 1;

        foreach (var lobbySlot in lobbySlots)
        {
            if (PlayerDataManager.instance == null || PlayerDataManager.instance.playerDatas == null ||
                reverseIndex >= PlayerDataManager.instance.playerDatas.Count) continue;
            
            PlayerDataManager.PlayerData currentPlayerData = PlayerDataManager.instance.playerDatas[reverseIndex];
            lobbySlot.UpdateSlotWithPlayerData(currentPlayerData.ID, currentPlayerData.IsReady);
            reverseIndex -= 1;
        }
    }

    [ClientRpc]
    private void RemovePlayerFromLobbyClientRpc()
    {
        if (!IsHost) return;
        PlayerDataManager.instance.RemovePlayerData(networkManager.LocalClientId);
        UpdateLobbySlotListClientRpc();
    }

    public void StartGame() => NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);

    public void ReadyButton() => ToggleReadyServerRpc(networkManager.LocalClientId);

    [ServerRpc(RequireOwnership = false)]
    private void ToggleReadyServerRpc(ulong _clientId)
    {
        if (!IsHost) return;

        foreach (var playerData in PlayerDataManager.instance.playerDatas)
        {
            if (playerData.ID != _clientId) continue;
            PlayerDataManager.instance.UpdatePlayerData(_clientId, !playerData.IsReady);
        }

        ToggleStartButton();
    }

    private void ToggleStartButton()
    {
        bool active = false;
        foreach (var playerData in PlayerDataManager.instance.playerDatas)
        {
            if (!playerData.IsReady)
            {
                active = false;
                break;
            }

            active = true;
        }

        startGameButton.gameObject.SetActive(active);
    }

    public void LeaveLobby()
    {
        if (!IsHost)
            SceneManager.LoadScene("MainMenu");
        networkManager.Shutdown();
        DontDestroyOnLoadController.DestroyAll();
    }
}