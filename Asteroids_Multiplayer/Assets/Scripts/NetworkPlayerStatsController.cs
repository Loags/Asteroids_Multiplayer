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

        // Spawn RankingSlots
        InitializeSlotsServerRpc();
    }

    [ServerRpc]
    private void InitializeSlotsServerRpc()
    {
        foreach (var currentPlayerData in PlayerDataManager.instance.playerDatas)
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

        SortPlayerDataByRankingClientRpc();
    }


    /// <summary>
    /// Updates all slots but not the local one (On Top)
    /// </summary>
    private void UpdateOtherSlotsData()
    {
        GameObject slotHolder = GameObject.FindGameObjectWithTag("RankingSlotHolder");
        slots = slotHolder.GetComponentsInChildren<NetworkPlayerStatsSlot>().ToList();
        if (slots.Count <= 0) return;

        int index = 0;

        foreach (var playerData in sortedDatas)
        {
            if (playerData.ID == NetworkManager.Singleton.LocalClientId) continue;

            PlayerDataManager.PlayerData currentPlayerData = playerData;
            slots[index].UpdateSlotData(currentPlayerData.ID, currentPlayerData.Points,
                currentPlayerData.Level,
                currentPlayerData.Rank);
            index += 1;
        }
    }


    /// <summary>
    /// Only updates the local slot
    /// </summary>
    private void UpdateLocalSlot()
    {
        foreach (var currentPlayerData in sortedDatas)
        {
            if (currentPlayerData.ID != NetworkManager.Singleton.LocalClientId) continue;
            localSlot.UpdateSlotData(currentPlayerData.ID, currentPlayerData.Points,
                currentPlayerData.Level,
                currentPlayerData.Rank);
            break;
        }
    }

    [ClientRpc]
    private void SortPlayerDataByRankingClientRpc()
    {
        SortPlayerDataByRanking();
    }

    public void SortPlayerDataByRanking()
    {
        sortedDatas.Clear();
        foreach (PlayerDataManager.PlayerData playerData in PlayerDataManager.instance.playerDatas)
        {
            sortedDatas.Add(playerData);
        }

        // Sort descending by points
        sortedDatas.Sort((x, y) => y.Points.CompareTo(x.Points));

        // Update the Rank based on the sorted order
        for (int i = 0; i < sortedDatas.Count; i++)
        {
            PlayerDataManager.PlayerData updatedPlayerData = sortedDatas[i];
            updatedPlayerData.Rank = i + 1;
            sortedDatas[i] = updatedPlayerData;
        }

        foreach (var playerData in sortedDatas)
        {
            Debug.Log("SortPlayerDataByRanking playerData ID: " + playerData.ID + "    level " + playerData.Level +
                      "    points " +
                      playerData.Points + "    rank " + playerData.Rank);
        }

        UpdateLocalSlot();
        UpdateOtherSlotsData();
    }
}