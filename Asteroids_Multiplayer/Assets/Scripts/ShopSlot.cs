using TMPro;
using UnityEngine;

public class ShopSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text priceText;
    public ShopItem shopItem;
    private ShopController shopController;

    public void UpdateSlot(ShopItem _shopItem, ShopController _shopController)
    {
        shopItem = _shopItem;
        descriptionText.text = _shopItem.description;
        priceText.text = "Costs: " + _shopItem.currentPrice + "pts.";
        shopController = _shopController;
    }

    public void BuyItem()
    {
        shopController.BuyItem(shopItem);
    }
}