using Unity.Netcode;
using UnityEngine;

public class NetworkObjectSpawner : NetworkBehaviour
{
    public static NetworkObjectSpawner Singleton;

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

    public static NetworkObject SpawnObjectByTypAtPosition(GameObject _prefabRef, Vector3 _position,
        Quaternion _rotation)
    {
        NetworkObject spawnedNetworkObject =
            NetworkObjectPool.Singleton.GetNetworkObject(_prefabRef, _position, _rotation);

        if (!spawnedNetworkObject.IsSpawned)
        {
            spawnedNetworkObject.gameObject.GetComponent<ObjectProperties>().ObjectDespawn +=
                NetworkWavesController.Singleton.ObjectStatusTracker;
            spawnedNetworkObject.Spawn();
        }


        return spawnedNetworkObject;
    }

    public void InstantDespawn(NetworkObject _networkObject, GameObject _prefabRef)
    {
        NetworkObjectPool.Singleton.ReturnNetworkObject(_networkObject, _prefabRef);
        if (_networkObject.IsSpawned)
        {
            ObjectProperties objectProperties = _networkObject.GetComponent<ObjectProperties>();
            objectProperties.OnDespawn();
            _networkObject.Despawn(false);
        }
    }
}