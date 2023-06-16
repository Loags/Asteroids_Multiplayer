using Unity.Netcode;
using UnityEngine;

public enum ObjectTyp
{
    Obstacle,
    Projectile
}

public class ObjectProperties : NetworkBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed;
    public float lifeSpan;
    public ObjectTyp typ;

    private Coroutine lifeCoroutine;


    public delegate void OnObjectDespawn(ObjectTyp _typ);

    public event OnObjectDespawn ObjectDespawn;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnDespawn()
    {
        ObjectDespawn?.Invoke(typ);
    }

    public void DespawnObject()
    {
        NetworkObjectSpawner.Singleton.InstantDespawn(gameObject.GetComponent<NetworkObject>(),
            NetworkObjectPool.Singleton.GetPrefabRef(typ));
    }
}