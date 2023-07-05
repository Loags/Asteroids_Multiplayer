using Unity.Netcode;
using UnityEngine;

public enum ObjectTyp
{
    Obstacle,
    Projectile,
    PickUp
}

public class ObjectProperties : NetworkBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed;
    public ObjectTyp typ;

    public delegate void OnObjectDespawn(ObjectTyp _typ, bool _increase);

    public event OnObjectDespawn ObjectDespawn;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnDespawn()
    {
        ObjectDespawn?.Invoke(typ, false);
    }

    protected void DespawnObject() => NetworkObjectSpawner.Singleton.InstantDespawn(
        gameObject.GetComponent<NetworkObject>(),
        NetworkObjectPool.Singleton.GetPrefabRef(typ));

    protected void LocalDespawn() => ObjectPool.instance.ReturnObjectToPool(gameObject);
}