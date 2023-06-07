using UnityEngine;

public class Projectile : ObjectProperties
{
    [SerializeField] private float damage;

    public float GetDamage => damage;

    public void Launch(Quaternion _rotation)
    {
        rb.velocity = _rotation * Vector2.up * moveSpeed;
    }
}