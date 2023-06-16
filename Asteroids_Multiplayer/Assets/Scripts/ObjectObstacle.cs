using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class ObjectObstacle : ObjectProperties
{
    [SerializeField] private float health;

    protected override void Awake()
    {
        base.Awake();
        typ = ObjectTyp.Obstacle;
    }

    private void FixedUpdate()
    {
        Vector3 velocity = new Vector3(0, -moveSpeed);
        rb.velocity = velocity;

        if (transform.position.y <= -10)
            DespawnObject();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Projectile")) return;

        ObjectProjectile objectProjectile = col.GetComponent<ObjectProjectile>();
        TakeDamage(objectProjectile.GetDamage);
    }

    private void TakeDamage(float _damage)
    {
        health -= _damage;
        if (!(health <= 0)) return;

        DespawnObject();
    }
}