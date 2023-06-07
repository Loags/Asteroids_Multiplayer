using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ObjectProperties : NetworkBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed;
    public float lifeSpan;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        DespawnClientRpc(lifeSpan);
    }

    [ClientRpc]
    public void DespawnClientRpc(float _lifeSpan = 0)
    {
        if (IsHost)
            Invoke(nameof(DespawnObject), _lifeSpan);
    }

    private void DespawnObject() => gameObject.GetComponent<NetworkObject>().Despawn();
}