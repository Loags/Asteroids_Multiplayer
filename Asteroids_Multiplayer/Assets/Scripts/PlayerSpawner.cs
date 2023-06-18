using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    public GameObject playerPrefab;
    [SerializeField] private List<Transform> spawnPoints;

    private Transform spawnPoint;

    private void Start()
    {
        SpawnPlayerPrefabServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerPrefabServerRpc(ulong _clientId)
    {
        for (int i = 0; i < PlayerDataManager.instance.playerDatas.Count; i++)
        {
            if (PlayerDataManager.instance.playerDatas[i].ID == _clientId)
                spawnPoint = spawnPoints[i];
        }

        var playerObject = Instantiate(playerPrefab);
        playerObject.GetComponent<NetworkObject>().SpawnWithOwnership(_clientId);
        playerObject.transform.position = spawnPoint.position;
    }
}