using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataManager : NetworkBehaviour
{
    /// <summary>
    /// Stores the player data of a client
    /// </summary>
    public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
    {
        public ulong ID;
        public bool IsReady;

        public PlayerData(ulong _id, bool _isReady)
        {
            ID = _id;
            IsReady = _isReady;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ID);
            serializer.SerializeValue(ref IsReady);
        }

        public bool Equals(PlayerData other)
        {
            return ID == other.ID;
        }
    }


    public static PlayerDataManager instance;

    /// <summary>
    /// Network Variables
    /// <param name="playerDatas"></param> Stores the data of each client that is connected
    /// </summary>
    public NetworkList<PlayerData> playerDatas;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(this);

        playerDatas = new NetworkList<PlayerData>();
    }

    public override void OnNetworkSpawn()
    {
        playerDatas.OnListChanged += DebugNetworkPlayerData;
        playerDatas.OnListChanged += OnPlayerDatasChanged;
        AddNewPlayerDataServerRpc(NetworkManager.Singleton.LocalClientId, false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddNewPlayerDataServerRpc(ulong _clientId, bool _isReady)
    {
        PlayerData newPlayerData = new PlayerData(_clientId, _isReady);

        bool playerDataAlreadyExists = false;

        foreach (var playerData in playerDatas)
        {
            if (playerData.ID == _clientId)
            {
                Debug.LogWarning("Trying to add data with already existing Client id: " + _clientId);
                playerDataAlreadyExists = true;
                break;
            }
        }

        if (playerDataAlreadyExists) return;

        playerDatas.Add(newPlayerData);
    }

    /// <summary>
    /// Add and remove lobby slots depending on connect or disconnect
    /// </summary>
    /// <param name="changeevent"></param> Check if disconnect of connect
    /// <exception cref="NotImplementedException"></exception>
    private void OnPlayerDatasChanged(NetworkListEvent<PlayerData> changeevent)
    {
        PlayerData modifiedPlayerData = new(changeevent.Value.ID, changeevent.Value.IsReady);
        switch (changeevent.Type)
        {
            case NetworkListEvent<PlayerData>.EventType.Add:
                // An item was added to the list
                Debug.Log("PlayerData added: ID = " + modifiedPlayerData.ID + ", IsReady = " +
                          modifiedPlayerData.IsReady);

                if (IsHost && LobbyManager.instance != null)
                {
                    LobbyManager.instance.AddLobbySlotServerRpc();
                }

                break;

            case NetworkListEvent<PlayerData>.EventType.Remove:
                // An item was removed from the list
                Debug.Log("PlayerData removed: ID = " + modifiedPlayerData.ID + ", IsReady = " +
                          modifiedPlayerData.IsReady);
                break;

            case NetworkListEvent<PlayerData>.EventType.Value:
                Debug.Log("PlayerDataList has been Modified");
                break;
        }
    }


    public void UpdatePlayerData(ulong _clientId, bool _isReady)
    {
        if (!IsHost) return;

        for (int i = 0; i < playerDatas.Count; i++)
        {
            if (playerDatas[i].ID != _clientId) continue;

            PlayerData currentPlayerData = playerDatas[i];
            currentPlayerData.IsReady = _isReady;
            playerDatas[i] = currentPlayerData;
        }
    }

    private void DebugNetworkPlayerData(NetworkListEvent<PlayerData> changeevent)
    {
        string output = "PlayerDatas - Amount: " + playerDatas.Count;
        foreach (var playerData in playerDatas)
        {
            output += "\nPlayerID: " + playerData.ID + "\nPlayerIsReady: " + playerData.IsReady + "\n";
        }

        Debug.Log(output);
    }

    /*private void OnSomeValueChanged(NetworkListEvent<ulong> changeevent)
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
*/
    public void RemovePlayerData(ulong _playerId)
    {
        foreach (var lobbySlot in LobbyManager.instance.lobbySlots)
        {
            if (lobbySlot.playerId != _playerId) continue;

            Destroy(lobbySlot.gameObject);
            break;
        }

        for (int i = 0; i < playerDatas.Count; i++)
        {
            if (playerDatas[i].ID != _playerId) continue;

            playerDatas.RemoveAt(i);
            break;
        }
    }
}