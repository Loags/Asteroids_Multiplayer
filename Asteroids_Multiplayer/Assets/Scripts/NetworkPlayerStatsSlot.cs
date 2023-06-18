using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerStatsSlot : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerLevel;
    [SerializeField] private TMP_Text playerPoints;
    [SerializeField] private TMP_Text playerRank;
    public ulong playerID;

    public void UpdateSlotData(ulong _playerID, int _points, int _level, int _rank)
    {
        playerID = _playerID;
        playerLevel.text = "Lv. " + _level;
        playerPoints.text = _points + " pts.";
        playerRank.text = "r. " + _rank;
    }
}