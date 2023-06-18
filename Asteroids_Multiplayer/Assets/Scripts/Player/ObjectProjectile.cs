using UnityEngine;

public class ObjectProjectile : ObjectProperties
{
    [SerializeField] private float damage;
    [SerializeField] private GameObject shootEffect;
    [SerializeField] private GameObject hitEffect;
    public ulong playerID;
    public float GetDamage => damage;

    protected override void Awake()
    {
        base.Awake();
        typ = ObjectTyp.Projectile;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Obstacle")) return;

        if (IsHost)
            DespawnObject();
    }

    private void FixedUpdate()
    {
        if (WorldSizeController.IsObjectOutsideWorldDimensions(transform.position))
            DespawnObject();
    }

    public void Launch(Quaternion _rotation, ulong _playerID)
    {
        playerID = _playerID;
        rb.velocity = (_rotation * Vector2.up * moveSpeed);
        Instantiate(shootEffect, transform.position, _rotation);
    }
}