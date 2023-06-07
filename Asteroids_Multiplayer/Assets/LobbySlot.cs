using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbySlot : NetworkBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image readyImage;
    [SerializeField] private TMP_Text nameText;

    public void UpdateSlotWithPlayerData(PlayerData _playerData)
    {
        nameText.text = "Player_" + _playerData.playerId.ToString();
        readyImage.color = !_playerData.isReady ? Color.red : Color.green;
    }
}