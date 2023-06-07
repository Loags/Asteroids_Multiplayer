using System.Collections.Generic;

public static class PlayerDataManager
{
    private static List<PlayerData> playerDatas;

    public static void AddPlayerData(PlayerData _playerData)
    {
        playerDatas.Add(_playerData);
    }

    public static void RemovePlayerData(ulong _playerId)
    {
        foreach (var playerData in playerDatas)
        {
            if (playerData.playerId != _playerId) continue;

            playerDatas.Remove(playerData);
            break;
        }
    }
}