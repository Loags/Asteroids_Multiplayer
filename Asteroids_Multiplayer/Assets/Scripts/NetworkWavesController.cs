using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkWavesController : NetworkBehaviour
{
    public static NetworkWavesController Singleton { get; private set; }

    [SerializeField] private float spawnIntervall;
    [SerializeField] private int maxAmountObstacles;
    private NetworkVariable<int> amountAlive = new NetworkVariable<int>();
    private Coroutine spawnCoroutine;

    public void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        NetworkObjectPool.InstatiatePoolDone += StartSpawningWithCoroutine;
    }

    public void ObjectStatusTracker(ObjectTyp _typ)
    {
        switch (_typ)
        {
            case ObjectTyp.Obstacle:
                amountAlive.Value -= 1;
                break;
            case ObjectTyp.Projectile:
                break;
            case ObjectTyp.PickUp:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_typ), _typ, null);
        }
    }

    [ServerRpc]
    private void SpawnObjectServerRpc()
    {
        NetworkObstacleController.Singleton.SpawnObstacles(1);
        amountAlive.Value += 1;
    }

    private void StartSpawningWithCoroutine()
    {
        spawnCoroutine = StartCoroutine(SpawnObjectsCoroutine());
    }

    private void StopSpawningWithCoroutine()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnObjectsCoroutine()
    {
        while (true)
        {
            if (amountAlive.Value < maxAmountObstacles)
            {
                SpawnObjectServerRpc();
            }

            yield return new WaitForSeconds(spawnIntervall);
        }
    }
}