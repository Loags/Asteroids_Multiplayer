using UnityEngine;

public class ObjectPickUpPoints : NetworkPickUp
{
    [SerializeField] private int minAmount;
    [SerializeField] private int maxAmount;

    protected override void Awake()
    {
        base.Awake();
        pickUpTyp = PickUpTyp.Points;
    }

    protected override void ApplyEffect()
    {
        PlayerDataManager.instance.ChangePointsServerRpc(playerController.playerID, RandomAmount());
        base.ApplyEffect();
    }

    private int RandomAmount()
    {
        return Random.Range(minAmount, maxAmount + 1);
    }
}