using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkWavesController : NetworkBehaviour
{
    public static NetworkWavesController Singleton { get; private set; }

    [SerializeField] private float spawnIntervall;
    [SerializeField] private int maxAmountObstacles;
    private NetworkVariable<int> amountAlive = new();

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

    public void ObstacleAmountStatusTracker(ObjectTyp _typ, bool _increase)
    {
        if (_typ != ObjectTyp.Obstacle) return;

        if (_increase)
            amountAlive.Value += 1;
        else
            amountAlive.Value -= 1;
    }

    [ServerRpc]
    private void SpawnObjectServerRpc()
    {
        NetworkObstacleController.Singleton.SpawnObstacles(1);
    }

    private void StartSpawningWithCoroutine() => StartCoroutine(SpawnObjectsCoroutine());

    private IEnumerator SpawnObjectsCoroutine()
    {
        while (true)
        {
            if (amountAlive.Value < maxAmountObstacles)
                SpawnObjectServerRpc();

            yield return new WaitForSeconds(spawnIntervall);
        }
    }
}