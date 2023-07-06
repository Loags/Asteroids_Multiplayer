using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ShipSlot : NetworkBehaviour
{
    [SerializeField] private Image border;
    [SerializeField] private Color highlight;
    [SerializeField] private Color normal;

    [SerializeField] private GameObject button;
    [SerializeField] private int shipIndex;

    public void Select()
    {
        ShipSlotController.instance.ToggleSelected();
        button.SetActive(false);
        border.color = highlight;
        PlayerDataManager.instance.ChangeShipSelectedIndexServerRpc(NetworkManager.Singleton.LocalClientId, shipIndex);
    }

    public void Deselect()
    {
        button.SetActive(true);
        border.color = normal;
    }
}