using Unity.Netcode;
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

        LocalDespawn();
    }

    private void FixedUpdate()
    {
        if (WorldSizeController.IsObjectOutsideWorldDimensions(transform.position))
            LocalDespawn();
    }

    public void Launch(Quaternion _rotation, ulong _playerID, float _damage)
    {
        damage = _damage;
        playerID = _playerID;
        rb.velocity = (_rotation * Vector2.up * moveSpeed);
        Instantiate(shootEffect, transform.position, _rotation).GetComponent<NetworkObject>().Spawn();
    }

    public void LocalLaunch(Quaternion _rotation, ulong _playerID, float _damage)
    {
        damage = _damage;
        playerID = _playerID;
        rb.velocity = (_rotation * Vector2.up * moveSpeed);
        Instantiate(shootEffect, transform.position, _rotation);
    }
}