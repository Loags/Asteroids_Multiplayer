using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerStatsController : NetworkBehaviour
{
    public static NetworkPlayerStatsController instance;
    [SerializeField] private GameObject playerStatsSlotPrefab;
    [SerializeField] private Transform targetPos;
    [SerializeField] private NetworkPlayerStatsSlot localSlot;
    private List<NetworkPlayerStatsSlot> slots = new();
    private List<PlayerDataManager.PlayerData> sortedDatas = new();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsHost) return;
        InitializeSlotsServerRpc();
        SortPlayerDataByRanking();
        UpdateLocalSlot();
    }

    [ServerRpc]
    private void InitializeSlotsServerRpc()
    {
        foreach (var currentPlayerData in sortedDatas)
        {
            if (currentPlayerData.ID == NetworkManager.Singleton.LocalClientId) continue;

            GameObject spawnedPrefab = Instantiate(playerStatsSlotPrefab);
            NetworkPlayerStatsSlot currentPlayerStatsSlot = spawnedPrefab.GetComponent<NetworkPlayerStatsSlot>();
            currentPlayerStatsSlot.UpdateSlotData(currentPlayerData.ID, currentPlayerData.Points,
                currentPlayerData.Level,
                currentPlayerData.Rank);
            slots.Add(currentPlayerStatsSlot);

            spawnedPrefab.GetComponent<NetworkObject>().Spawn();
            spawnedPrefab.transform.SetParent(targetPos);
        }

        UpdateSlotsDataClientRpc();
    }


    [ClientRpc]
    public void UpdateSlotsDataClientRpc()
    {
        slots = FindObjectsOfType<NetworkPlayerStatsSlot>().ToList();
        int reverseIndex = slots.Count - 1;

        foreach (var networkPlayerStatsSlot in slots)
        {
            if (networkPlayerStatsSlot.playerID == NetworkManager.Singleton.LocalClientId) continue;

            PlayerDataManager.PlayerData currentPlayerData = sortedDatas[reverseIndex];
            networkPlayerStatsSlot.UpdateSlotData(currentPlayerData.ID, currentPlayerData.Points,
                currentPlayerData.Level,
                currentPlayerData.Rank);
            reverseIndex -= 1;
        }
    }

    public void UpdateLocalSlot()
    {
        Debug.Log("UpdateLocalSlot");
        foreach (var currentPlayerData in sortedDatas)
        {
            if (currentPlayerData.ID != NetworkManager.Singleton.LocalClientId) continue;
            localSlot.UpdateSlotData(currentPlayerData.ID, currentPlayerData.Points,
                currentPlayerData.Level,
                currentPlayerData.Rank);
            break;
        }
    }

    public void SortPlayerDataByRanking()
    {
        foreach (PlayerDataManager.PlayerData playerData in PlayerDataManager.instance.playerDatas)
        {
            sortedDatas.Add(playerData);
        }

        sortedDatas.Sort(new PlayerDataComparer());
        Debug.Log("SortedDataList");
    }
}

public class PlayerDataComparer : IComparer<PlayerDataManager.PlayerData>
{
    public int Compare(PlayerDataManager.PlayerData x, PlayerDataManager.PlayerData y)
    {
        // Compare the ranks of the player data
        return x.Rank.CompareTo(y.Rank);
    }
}