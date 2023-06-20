using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkObstacleController : NetworkBehaviour
{
    public static NetworkObstacleController Singleton { get; private set; }

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

    private void SpawnObstacles()
    {
        GameObject prefabToSpawn = ChooseRandomPrefab();
        NetworkObjectSpawner.SpawnObjectByTypAtPosition(prefabToSpawn,
            WorldSizeController.GenerateSpawnPoint(),
            quaternion.identity);
    }
    
    private GameObject ChooseRandomPrefab()
    {
        int randomIndex = Random.Range(0, prefabsToSpawn.Count);
        return prefabsToSpawn[randomIndex];
    }
}