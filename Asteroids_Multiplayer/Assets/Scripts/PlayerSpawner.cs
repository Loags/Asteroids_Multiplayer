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
        Debug.Log("Try Spawning");
        //SpawnPlayerPrefabServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    /*private void Spawn()
    {
        foreach (var playerId in PlayerDataManager.instance.playerIds)
        {
            SpawnPlayerPrefabServerRpc(playerId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerPrefabServerRpc(ulong _clientId)
    {
        for (int i = 0; i < PlayerDataManager.instance.playerIds.Count; i++)
        {
            if (PlayerDataManager.instance.playerIds[i] == _clientId)
                spawnPoint = spawnPoints[i];
        }

        Debug.Log("Spawning player");
        var playerObject = Instantiate(playerPrefab);
        playerObject.GetComponent<NetworkObject>().SpawnWithOwnership(_clientId);
        playerObject.transform.position = spawnPoint.position;

        if (_clientId == NetworkManager.Singleton.LocalClientId)
        {
            // Enable local client control
            //playerObject.GetComponent<PlayerController>().EnableLocalControl();
        }
    }*/
}