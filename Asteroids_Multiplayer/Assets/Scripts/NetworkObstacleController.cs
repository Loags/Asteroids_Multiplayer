using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkObstacleController : NetworkBehaviour
{
    public static NetworkObstacleController Singleton { get; private set; }

    [Header("Spawn area for Obstacles")] [SerializeField]
    private float minX = -10f;

    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    [Space(20)] [SerializeField] private List<GameObject> prefabsToSpawn;

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
    
    public void SpawnObstacles(int amount)
    {
        for (int i = 0; i < amount; i++)
            SpawnObstacles();
    }

    [ServerRpc]
    private void SpawnObstaclesServerRpc()
    {
        SpawnObstacles();
    }

    private async void SpawnObstacles()
    {
        GameObject prefabToSpawn = ChooseRandomPrefab();
        NetworkObject spawnedNetworkObject = NetworkObjectSpawner.SpawnObjectByTypAtPosition(prefabToSpawn,
            GenerateSpawnPoint(),
            quaternion.identity);

        ObjectProperties objectProperties = spawnedNetworkObject.gameObject.GetComponent<ObjectProperties>();

        // await NetworkObjectSpawner.Singleton.WaitThenDespawn(spawnedNetworkObject, prefabToSpawn,
        //     objectProperties.lifeSpan);
    }

    private Vector3 GenerateSpawnPoint()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        return new Vector3(randomX, randomY, 0f);
    }

    private GameObject ChooseRandomPrefab()
    {
        int randomIndex = Random.Range(0, prefabsToSpawn.Count);
        return prefabsToSpawn[randomIndex];
    }
}