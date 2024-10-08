using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;


[Serializable]
public class ShopItem
{
    public EShopItemTyp itemTyp;
    public string description;
    public float currentValue;
    public int currentPrice;

    [Header("Value between 0-1")] public float increasePriceByPercent;
}

public enum EShopItemTyp
{
    MaxHealth,
    CurrentHealth,
    DamageBuff,
    ShootingSpeed,
    RotationSpeed,
    MovementSpeed
}

public class ShopController : MonoBehaviour
{
    public static ShopController instance;
    [SerializeField] private GameObject shopSlotPrefab;
    [SerializeField] private List<ShopSlot> shopSlots;
    [SerializeField] private List<ShopItem> shopItems;
    private PlayerController playerController;
    private GameObject workAroundGameObject;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        InitializeShop();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        playerController = GetterPlayerController.GetPlayerControllerWithID(NetworkManager.Singleton.LocalClientId);
    }

    private void InitializeShop()
    {
        GameObject shopTargetAnchor = GameObject.FindGameObjectWithTag("ShopSlotHolder");

        foreach (var shopItem in shopItems)
        {
            GameObject spawnedShopSlotObject = Instantiate(shopSlotPrefab, shopTargetAnchor.transform);
            ShopSlot spawnedShopSlot = spawnedShopSlotObject.GetComponent<ShopSlot>();
            shopSlots.Add(spawnedShopSlot);
            spawnedShopSlot.UpdateSlot(shopItem, this);
        }

        workAroundGameObject = transform.GetChild(0).gameObject;
        ToggleShop();
    }

    private void UpdateSlots()
    {
        for (int i = 0; i < shopSlots.Count; i++)
        {
            shopSlots[i].UpdateSlot(shopItems[i], this);
        }
    }

    public void BuyItem(ShopItem _item)
    {
        if (GetCurrentPoints() < _item.currentPrice) return;

        switch (_item.itemTyp)
        {
            case EShopItemTyp.MaxHealth:
                IncreaseMaxHealth();
                break;
            case EShopItemTyp.CurrentHealth:
                // Cancel out if currentHealth cant be increased due to maxHealth reached
                if (!CanIncreaseCurrentHealth()) return;
                break;
            case EShopItemTyp.DamageBuff:
                IncreaseDamage((int)_item.currentValue);
                break;
            case EShopItemTyp.ShootingSpeed:
                IncreaseShootingSpeed(_item.currentValue);
                break;
            case EShopItemTyp.RotationSpeed:
                IncreaseRotationSpeed(_item.currentValue);
                break;
            case EShopItemTyp.MovementSpeed:
                IncreaseMoveSpeed(_item.currentValue);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        PlayerDataManager.instance.ChangePointsServerRpc(NetworkManager.Singleton.LocalClientId, -_item.currentPrice);
        PlayerDataManager.instance.ChangeLevelServerRpc(NetworkManager.Singleton.LocalClientId);

        // Increase price for next buy
        foreach (var shopItem in shopItems)
        {
            if (shopItem.itemTyp != _item.itemTyp) continue;
            int newPrice = (int)(shopItem.currentPrice * (1 + _item.increasePriceByPercent));
            shopItem.currentPrice = newPrice;
        }

        UpdateSlots();
    }

    private int GetCurrentPoints()
    {
        int currentPoints = -1;
        foreach (var playerData in PlayerDataManager.instance.playerDatas)
        {
            if (playerData.ID != NetworkManager.Singleton.LocalClientId) continue;
            currentPoints = playerData.Points;
        }

        return currentPoints;
    }

    private void IncreaseMaxHealth() => playerController.playerHealthController.IncreaseMaxHealth();

    private bool CanIncreaseCurrentHealth()
    {
        return playerController.playerHealthController.IncreaseCurrentHealth();
    }

    private void IncreaseDamage(int _value) => playerController.shootingController.IncreaseDamage(_value);

    private void IncreaseShootingSpeed(float _value) =>
        playerController.shootingController.IncreaseShootingSpeed(_value);

    private void IncreaseRotationSpeed(float _value) =>
        playerController.movementController.IncreaseRotationSpeed(_value);

    private void IncreaseMoveSpeed(float _value) =>
        playerController.movementController.IncreaseMoveSpeed(_value);

    public void ToggleShop() => workAroundGameObject.SetActive(!workAroundGameObject.activeSelf);
}