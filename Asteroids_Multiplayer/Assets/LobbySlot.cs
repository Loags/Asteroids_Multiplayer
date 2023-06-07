using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySlot : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image readyImage;
    [SerializeField] private TMP_Text nameText;

    public void UpdateSlotWithPlayerData()
    {
        PlayerData playerData = GetComponent<PlayerData>();
        nameText.text = "Player_" + playerData.playerId.ToString();
        readyImage.color = !playerData.isReady ? Color.red : Color.green;
    }
}