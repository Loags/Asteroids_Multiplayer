using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectObstacle : ObjectProperties
{
    [SerializeField] private float health;
    [SerializeField] private int points;
    private float minMoveSpeed;
    private float maxMoveSpeed;

    protected override void Awake()
    {
        base.Awake();
        typ = ObjectTyp.Obstacle;
    }

    private void OnEnable()
    {
        minMoveSpeed = moveSpeed / 2;
        maxMoveSpeed = moveSpeed * 2;
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
    }

    private void FixedUpdate()
    {
        Vector3 velocity = new Vector3(0, -moveSpeed);
        rb.velocity = velocity;

        if (WorldSizeController.IsObjectOutsideWorldDimensions(transform.position))
            DespawnObject();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Projectile")) return;

        ObjectProjectile objectProjectile = col.GetComponent<ObjectProjectile>();
        TakeDamage(objectProjectile.playerID, objectProjectile.GetDamage);
    }

    private void TakeDamage(ulong _playerId, float _damage)
    {
        health -= _damage;
        if (!(health <= 0)) return;

        PlayerDataManager.instance.ChangePointsServerRpc(_playerId, points);
        DespawnObject();
    }
}