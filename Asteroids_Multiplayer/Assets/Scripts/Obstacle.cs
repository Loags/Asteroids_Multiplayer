using UnityEngine;

public class Obstacle : ObjectProperties
{
    [SerializeField] private float health;


    private void FixedUpdate()
    {
        Vector3 velocity = new Vector3(0, -moveSpeed);
        rb.velocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Projectile")) return;

        Projectile projectile = col.GetComponent<Projectile>();
        TakeDamage(projectile.GetDamage);
        projectile.DespawnClientRpc();
    }

    private void TakeDamage(float _damage)
    {
        health -= _damage;
        if (health <= 0)
            DespawnClientRpc();
    }
}