using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerDataManager : NetworkBehaviour
{
    public static PlayerDataManager instance;

    /// <summary>
    /// Network Variables
    /// </summary>
    public NetworkList<ulong> playerIds;

    public List<LobbySlot> slots = new();

    private void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(this);
        playerIds = new NetworkList<ulong>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(UpdateSlotsClientRpc), 0f, 1f);
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

    public void DebugNetworkIdList()
    {
        string output = "PlayerIds - Amount: " + playerIds.Count + "\n";
        foreach (var playerId in playerIds)
        {
            output += "\nPlayerID: " + playerId + "\n";
        }

        Debug.Log(output);
    }

    public void AddPlayerData(ulong _playerId, LobbySlot _slot)
    {
        slots.Add(_slot);
        playerIds.Add(_playerId);
        DebugNetworkIdList();
    }

    public void RemovePlayerData(ulong _playerId)
    {
        LobbySlot[] lobbySlots = FindObjectsOfType<LobbySlot>();
        foreach (var lobbySlot in lobbySlots)
        {
            PlayerData playerData;
            if (lobbySlot.gameObject.TryGetComponent(out playerData))
            {
                Destroy(lobbySlot.gameObject);
                break;
            }
        }
    }

    [ClientRpc]
    private void UpdateSlotsClientRpc()
    {
        Debug.Log("UpdateSlots");
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].UpdateSlotWithPlayerData(playerIds[i]);
        }
    }
}