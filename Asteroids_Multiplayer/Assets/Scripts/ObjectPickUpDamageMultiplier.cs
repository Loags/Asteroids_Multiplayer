using TMPro;
using UnityEngine;

public class ObjectPickUpDamageMultiplier : NetworkPickUp
{
    [SerializeField] private float durationMultiplier;
    [SerializeField] private float minMultiplier;
    [SerializeField] private float maxMultiplier;
    [SerializeField] private TMP_Text textDisplay;

    private float multiplier;

    protected override void Awake()
    {
        base.Awake();
        pickUpTyp = PickUpTyp.DamageMultiplier;
    }

    private void OnEnable()
    {
        multiplier = RandomAmount();
        textDisplay.text = "Damage\nx " + multiplier;
    }

    protected override void ApplyEffect()
    {
        PlayerDataManager.instance.ToggleDamageMultiplier(durationMultiplier, multiplier);
        base.ApplyEffect();
    }

    private float RandomAmount()
    {
        return Mathf.Round(Random.Range(minMultiplier, maxMultiplier + 1) * 10f) / 10f;
    }
}