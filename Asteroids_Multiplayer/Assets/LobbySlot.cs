using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbySlot : NetworkBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image readyImage;
    [SerializeField] private TMP_Text nameText;
    public ulong playerId;
    public bool isPlayerReady;


    public void InitializeSlotData(ulong _playerId)
    {
        playerId = _playerId;
        readyImage.color = Color.red;
    }

    public void UpdateSlotWithPlayerData(ulong _playerId, bool _isPlayerReady)
    {
        playerId = _playerId;
        isPlayerReady = _isPlayerReady;
        nameText.text = "Player_" + playerId;
        readyImage.color = isPlayerReady ? Color.green : Color.red;
    }
}