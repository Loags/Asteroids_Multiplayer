using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerDataManager : NetworkBehaviour
{
    public static PlayerDataManager instance;

    /// <summary>
    /// Network Variables
    /// </summary>
    public NetworkList<ulong> playerIds;

    public NetworkList<bool> isPlayerReady;

    public List<LobbySlot> lobbySlots = new();

    private void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(this);
        playerIds = new NetworkList<ulong>();
        isPlayerReady = new NetworkList<bool>();
    }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += UpdateSlots;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) return;

        playerIds.OnListChanged += OnSomeValueChanged;
    }

    private void OnSomeValueChanged(NetworkListEvent<ulong> changeevent)
    {
        DebugNetworkIdList();
    }

    private void DebugNetworkIdList()
    {
        string output = "PlayerIds - Amount: " + playerIds.Count + "\n";
        foreach (var playerId in playerIds)
        {
            output += "\nPlayerID: " + playerId + "\n";
        }

        Debug.Log(output);
    }

    public void AddPlayerData(ulong _playerId)
    {
        playerIds.Add(_playerId);
        isPlayerReady.Add(false);
        DebugNetworkIdList();
    }

    public void RemovePlayerData(ulong _playerId)
    {
        lobbySlots = FindObjectsOfType<LobbySlot>().ToList();

        foreach (var lobbySlot in lobbySlots)
        {
            if (lobbySlot.playerId != _playerId) continue;

            Destroy(lobbySlot.gameObject);
            break;
        }
    }

    private void UpdateSlots()
    {
        if (IsHost)
            InvokeRepeating(nameof(UpdateSlotsServerRpc), 0f, 1f);
    }

    [ServerRpc]
    private void UpdateSlotsServerRpc()
    {
        UpdateSlotsClientRpc();
    }

    [ClientRpc]
    private void UpdateSlotsClientRpc()
    {
        int reverseIndex = lobbySlots.Count - 1;
        foreach (var lobbySlot in lobbySlots)
        {
            lobbySlot.UpdateSlotWithPlayerData(playerIds[reverseIndex], isPlayerReady[reverseIndex]);

            reverseIndex -= 1;
        }
    }
}