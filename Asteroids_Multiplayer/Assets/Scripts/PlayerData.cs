using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public ulong playerId;
    public bool isReady;

    public void SetPlayerData(ulong _playerId)
    {
        playerId = _playerId;
    }
}