using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance;
    [SerializeField] private List<PlayerData> playerDatas = new();
    private void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(this);
    }

    public void ClearPlayerData() => playerDatas.Clear();

    public void AddPlayerData(PlayerData _playerData)
    {
        playerDatas.Add(_playerData);
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


        foreach (var playerData in playerDatas)
        {
            if (playerData.playerId != _playerId) continue;

            playerDatas.Remove(playerData);
            break;
        }
    }
}