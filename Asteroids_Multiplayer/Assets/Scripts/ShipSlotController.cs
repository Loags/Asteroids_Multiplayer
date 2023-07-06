using System.Collections.Generic;
using UnityEngine;

public class ShipSlotController : MonoBehaviour
{
    public static ShipSlotController instance;
    [SerializeField] private List<ShipSlot> shipSlots;
    [SerializeField] private CanvasGroup lobbyCanvasGroup;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ToggleSelected()
    {
        foreach (var shipSlot in shipSlots)
        {
            shipSlot.Deselect();
        }
    }

    public void CloseShipSelection()
    {
        CanvasGroup ownCanvasGroup = GetComponent<CanvasGroup>();
        ownCanvasGroup.interactable = false;
        ownCanvasGroup.alpha = 0;
        ownCanvasGroup.blocksRaycasts = false;

        lobbyCanvasGroup.interactable = true;
        lobbyCanvasGroup.alpha = 1;
        lobbyCanvasGroup.blocksRaycasts = true;
    }
}