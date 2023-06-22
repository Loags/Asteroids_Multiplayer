using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPickUpController : NetworkBehaviour
{
    public static NetworkPickUpController Singleton;
    [SerializeField] private List<GameObject> prefabsToSpawn;
    [SerializeField] private int maxActivePickups = 3;
    [SerializeField] private float spawnInterval = 10f;

    public int activePickups;

    private void Awake()
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
        base.OnNetworkSpawn();
        if (!IsHost) return;

        StartCoroutine(SpawnPickupsCoroutine());
    }

    private IEnumerator SpawnPickupsCoroutine()
    {
        while (true)
        {
            if (activePickups < maxActivePickups)
            {
                ShowPickUpServerRpc();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }


    [ServerRpc]
    private void ShowPickUpServerRpc()
    {
        NetworkObjectSpawner.SpawnObjectByTypAtPosition(ChooseRandomPickUp(), GetRandomPositionWithinWorld(),
            Quaternion.identity);
        activePickups += 1;
    }

    private Vector3 GetRandomPositionWithinWorld()
    {
        int offSet = 5;
        float randomX = Random.Range(WorldSizeController.worldDimensionsMin.x + offSet,
            WorldSizeController.worldDimensionsMax.x - offSet);
        float randomY = Random.Range(WorldSizeController.worldDimensionsMin.y + offSet,
            WorldSizeController.worldDimensionsMax.y - offSet);
        return new Vector3(randomX, randomY, 0f);
    }

    private GameObject ChooseRandomPickUp()
    {
        return prefabsToSpawn[Random.Range(0, prefabsToSpawn.Count)];
    }
}