using UnityEngine;

public class PlayerData
{
    public ulong playerId;
    public int level;
    public int damageDone;
    

    public void SetPlayerData(ulong _playerId)
    {
        playerId = _playerId;
    }
}