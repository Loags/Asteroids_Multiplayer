using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectObstacle : ObjectProperties
{
    public Vector3 moveDir;
    [SerializeField] private float health;
    [SerializeField] private int points;

    [Header("Move speed will be divided by and multiplied with this value")] [SerializeField]
    private float moveSpeedModifier;

    [SerializeField] private float moveDirOffSet;

    private float minMoveSpeed;
    private float maxMoveSpeed;

    protected override void Awake()
    {
        base.Awake();
        typ = ObjectTyp.Obstacle;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        moveDir = GenerateMoveDir();
    }

    private void OnEnable()
    {
        minMoveSpeed = moveSpeed / moveSpeedModifier;
        maxMoveSpeed = moveSpeed * moveSpeedModifier;
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
    }

    private void FixedUpdate()
    {
        Vector3 velocity = moveDir * moveSpeed;
        rb.velocity = velocity;

        if (WorldSizeController.IsObjectOutsideWorldDimensions(transform.position) && IsHost)
            DespawnObject();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            ObjectProjectile objectProjectile = col.GetComponent<ObjectProjectile>();
            TakeDamage(objectProjectile.playerID, objectProjectile.GetDamage);
        }

        if (!col.gameObject.CompareTag("Player")) return;

        PlayerHealthController playerHealthController = col.GetComponent<PlayerHealthController>();
        playerHealthController.TakeDamage();
    }

    private void TakeDamage(ulong _playerId, float _damage)
    {
        health -= _damage;
        if (!(health <= 0)) return;

        PlayerDataManager.instance.ChangePointsServerRpc(_playerId, points);
        DespawnObject();
    }

    private Vector3 GenerateMoveDir()
    {
        Vector3 targetDirection = -transform.position;

        targetDirection.Normalize();

        // Add random offset to the move direction
        Vector3 randomOffset = Random.insideUnitCircle * moveDirOffSet;
        Vector3 moveDir = targetDirection + randomOffset;

        moveDir.Normalize();

        return moveDir;
    }
}