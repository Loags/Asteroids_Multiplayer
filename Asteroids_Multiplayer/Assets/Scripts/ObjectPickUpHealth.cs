public class ObjectPickUpHealth : NetworkPickUp
{
    protected override void Awake()
    {
        base.Awake();
        pickUpTyp = PickUpTyp.Health;
    }

    protected override void ApplyEffect()
    {
        if (playerController != null)
            playerController.playerHealthController.IncreaseCurrentHealth();
        base.ApplyEffect();
    }
}