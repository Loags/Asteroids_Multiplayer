using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Client.Photon.StructWrapping;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Analytics;

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

    private void Initialize()
    {
        SortPlayerDataByRankingClientRpc();
        UpdateLocalSlot();
    }

    [ServerRpc]
    private void InitializeSlotsServerRpc()
    {
        foreach (var currentPlayerData in PlayerDataManager.instance.playerDatas)
        {
            if (currentPlayerData.ID == NetworkManager.Singleton.LocalClientId) continue;

            GameObject spawnedPrefab = Instantiate(playerStatsSlotPrefab);
            NetworkPlayerStatsSlot currentPlayerStatsSlot = spawnedPrefab.GetComponent<NetworkPlayerStatsSlot>();
            Debug.Log("ID ON INITIALIZE SERVERRPC: " + currentPlayerData.ID);
            currentPlayerStatsSlot.UpdateSlotData(currentPlayerData.ID, currentPlayerData.Points,
                currentPlayerData.Level,
                currentPlayerData.Rank);
            slots.Add(currentPlayerStatsSlot);

            spawnedPrefab.GetComponent<NetworkObject>().Spawn();
            spawnedPrefab.transform.SetParent(targetPos);
        }

        foreach (var networkPlayerStatsSlot in slots)
        {
            Debug.Log("ID IN SERVERRPC: " + networkPlayerStatsSlot.playerID);
        }

        SortPlayerDataByRankingClientRpc();
    }


    /// <summary>
    /// Updates all slots but not the local one (On Top)
    /// </summary>
    private void UpdateOtherSlotsData()
    {
        Debug.Log("Update other slot data");

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

    [ClientRpc]
    public void SortPlayerDataByRankingClientRpc()
    {
        Debug.Log("SortPlayerDataByRanking");
        sortedDatas.Clear();
        foreach (PlayerDataManager.PlayerData playerData in PlayerDataManager.instance.playerDatas)
        {
            sortedDatas.Add(playerData);
        }


        // The rank calculation has to be locally because every change in the NetworkList
        // would call SortPlayerDataByRankingClientRpc and causes a StackOverflow
        for (int i = 0; i < sortedDatas.Count; i++)
        {
            PlayerDataManager.PlayerData updatedPlayerData = sortedDatas[i];
            updatedPlayerData.Rank = i + 1;
            sortedDatas[i] = updatedPlayerData;
        }

        sortedDatas.Sort(new PlayerDataComparer());

        Debug.Log("SortedList");
        foreach (var playerData in sortedDatas)
        {
            Debug.Log("playerData ID: " + playerData.ID + "    level " + playerData.Level + "    points " +
                      playerData.Points + "    rank " + playerData.Rank);
        }

        UpdateLocalSlot();
        UpdateOtherSlotsData();
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