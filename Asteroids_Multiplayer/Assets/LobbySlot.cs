using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbySlot : NetworkBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image readyImage;
    [SerializeField] private TMP_Text nameText;

    public void UpdateSlotWithPlayerData(ulong _playerId)
    {
        nameText.text = "Player_" + _playerId;
        //readyImage.color = !_playerId.isReady ? Color.red : Color.green;
    }
}