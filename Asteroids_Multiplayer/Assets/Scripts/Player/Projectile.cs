using UnityEngine;

public class Projectile : ObjectProperties
{
    [SerializeField] private float damage;

    public float GetDamage => damage;

    protected override void Awake()
    {
        base.Awake();
        typ = ObjectTyp.Projectile;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Obstacle")) return;

        DespawnObject();
    }

    public void Launch(Quaternion _rotation)
    {
        rb.velocity = (_rotation * Vector2.up * moveSpeed);
    }
}