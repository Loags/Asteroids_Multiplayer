using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerDataManager : NetworkBehaviour
{
    /// <summary>
    /// Stores the player data of a client
    /// </summary>
    public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
    {
        public ulong ID;
        public bool IsReady;
        public int Points;
        public int Level;
        public int Rank;
        public int SelectedShipIndex;

        public PlayerData(ulong _id, bool _isReady)
        {
            ID = _id;
            IsReady = _isReady;
            Points = 0;
            Level = 0;
            Rank = 0;
            SelectedShipIndex = 0;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ID);
            serializer.SerializeValue(ref IsReady);
            serializer.SerializeValue(ref Points);
            serializer.SerializeValue(ref Level);
            serializer.SerializeValue(ref Rank);
            serializer.SerializeValue(ref SelectedShipIndex);
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

    private bool pointsMultiplierActive;
    private float pointsMultiplier;
    private Coroutine pointsMultiplierCoroutine;

    [HideInInspector] public bool damageMultiplierActive;
    [HideInInspector] public float damageMultiplier;
    private Coroutine damageMultiplierCoroutine;


    private void Awake()
    {
        if (instance == null)
            instance = this;

        gameObject.AddToDontDestroyOnLoad();

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
            if (playerData.ID != _clientId) continue;

            playerDataAlreadyExists = true;
            break;
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
        switch (changeevent.Type)
        {
            case NetworkListEvent<PlayerData>.EventType.Add:
                if (IsHost && LobbyManager.instance != null)
                    LobbyManager.instance.AddLobbySlotServerRpc();
                break;
            case NetworkListEvent<PlayerData>.EventType.Remove:
                break;
            case NetworkListEvent<PlayerData>.EventType.Value:
                if (NetworkPlayerStatsController.instance != null)
                    NetworkPlayerStatsController.instance.SortPlayerDataByRanking();
                break;
        }
    }


    public void UpdateReadyPlayerData(ulong _clientId, bool _isReady)
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
            output += "\nPlayerID: " + playerData.ID + "\nPlayerIsReady: " + playerData.IsReady + "\nPlayerPoints: " +
                      playerData.Points + "\nShipIndex: " + playerData.SelectedShipIndex + "\n";
        }

        Debug.Log(output);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangePointsServerRpc(ulong _id, int _amount)
    {
        if (!IsHost) return;

        for (int i = 0; i < playerDatas.Count; i++)
        {
            if (playerDatas[i].ID != _id) continue;

            PlayerData updatedPlayerData = playerDatas[i];
            if (pointsMultiplierActive)
            {
                int modifiedAmount = (int)(_amount * pointsMultiplier);
                updatedPlayerData.Points += modifiedAmount;
            }
            else
                updatedPlayerData.Points += _amount;

            playerDatas[i] = updatedPlayerData;
            break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeLevelServerRpc(ulong _id)
    {
        if (!IsHost) return;

        for (int i = 0; i < playerDatas.Count; i++)
        {
            if (playerDatas[i].ID != _id) continue;

            PlayerData updatedPlayerData = playerDatas[i];
            updatedPlayerData.Level += 1;
            playerDatas[i] = updatedPlayerData;
            break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeShipSelectedIndexServerRpc(ulong _id, int _index)
    {
        if (!IsHost) return;

        for (int i = 0; i < playerDatas.Count; i++)
        {
            if (playerDatas[i].ID != _id) continue;
            PlayerData updatedPlayerData = playerDatas[i];
            updatedPlayerData.SelectedShipIndex = _index;
            playerDatas[i] = updatedPlayerData;
            break;
        }
    }

    public int GetShipIndexWithClientID(ulong _id)
    {
        foreach (var playerData in playerDatas)
        {
            if (playerData.ID != _id) continue;
            return playerData.SelectedShipIndex;
        }

        return -1;
    }

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

    public void TogglePointsMultiplier(float _duration, float _multiplier)
    {
        if (pointsMultiplier < _multiplier)
        {
            if (pointsMultiplierCoroutine != null)
                StopCoroutine(pointsMultiplierCoroutine);
            pointsMultiplierCoroutine = StartCoroutine(PointsMultiplierCoroutine(_duration));
            pointsMultiplier = _multiplier;
        }

        if (pointsMultiplierCoroutine != null) return;

        pointsMultiplier = _multiplier;
        pointsMultiplierCoroutine = StartCoroutine(PointsMultiplierCoroutine(_duration));
    }

    private IEnumerator PointsMultiplierCoroutine(float _duration)
    {
        PlayerBuffDisplayController playerBuffDisplayController = FindObjectOfType<PlayerBuffDisplayController>();
        playerBuffDisplayController.TogglePointsBuff(true, 0);
        pointsMultiplierActive = true;
        yield return new WaitForSeconds(_duration);
        pointsMultiplierActive = false;
        playerBuffDisplayController.TogglePointsBuff(false, 0);
        pointsMultiplierCoroutine = null;
    }

    public void ToggleDamageMultiplier(float _duration, float _multiplier)
    {
        if (damageMultiplier < _multiplier)
        {
            if (damageMultiplierCoroutine != null)
                StopCoroutine(damageMultiplierCoroutine);
            damageMultiplierCoroutine = StartCoroutine(DamageMultiplierCoroutine(_duration));
            damageMultiplier = _multiplier;
        }

        if (damageMultiplierCoroutine != null) return;

        damageMultiplier = _multiplier;
        damageMultiplierCoroutine = StartCoroutine(DamageMultiplierCoroutine(_duration));
    }

    private IEnumerator DamageMultiplierCoroutine(float _duration)
    {
        PlayerBuffDisplayController playerBuffDisplayController = FindObjectOfType<PlayerBuffDisplayController>();
        playerBuffDisplayController.TogglePointsBuff(true, 1);
        damageMultiplierActive = true;
        yield return new WaitForSeconds(_duration);
        playerBuffDisplayController.TogglePointsBuff(false, 1);
        damageMultiplierActive = false;
        damageMultiplierCoroutine = null;
    }
}