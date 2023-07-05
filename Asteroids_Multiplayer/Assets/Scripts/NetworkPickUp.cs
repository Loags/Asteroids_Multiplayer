using System;
using UnityEngine;


[Serializable]
public enum PickUpTyp
{
    Points,
    PointsMultiplier,
    Health,
    DamageMultiplier
}

public class NetworkPickUp : ObjectProperties
{
    public PickUpTyp pickUpTyp;

    protected PlayerController playerController;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        playerController = col.GetComponent<PlayerController>();

        ApplyEffect();
    }

    protected virtual void ApplyEffect()
    {
        if (!IsHost) return;

        NetworkPickUpController.Singleton.activePickups -= 1;
        DespawnObject();
    }
}