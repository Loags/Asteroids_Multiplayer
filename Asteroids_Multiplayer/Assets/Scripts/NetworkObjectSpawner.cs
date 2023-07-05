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

    /// <summary>
    /// Spawns a network object of a specific type at the given position and rotation.
    /// It gets the prefab to spawn from the NetworKObjectPool by GetNetworkObject
    /// </summary>
    /// <param name="_prefabRef">The prefab reference of the network object to spawn.</param>
    /// <param name="_position">The position to spawn the network object at.</param>
    /// <param name="_rotation">The rotation to spawn the network object with.</param>
    public static void SpawnObjectByTypAtPosition(GameObject _prefabRef, Vector3 _position,
        Quaternion _rotation)
    {
        NetworkObject spawnedNetworkObject =
            NetworkObjectPool.Singleton.GetNetworkObject(_prefabRef, _position, _rotation);

        if (spawnedNetworkObject.IsSpawned) return;

        ObjectProperties objectProperties = spawnedNetworkObject.gameObject.GetComponent<ObjectProperties>();
        objectProperties.ObjectDespawn += NetworkWavesController.Singleton.ObstacleAmountStatusTracker;
        NetworkWavesController.Singleton.ObstacleAmountStatusTracker(objectProperties.typ, true);

        spawnedNetworkObject.Spawn();
    }

    /// <summary>
    /// Instantly despawns a network object and returns it to the object pool.
    /// </summary>
    /// <param name="_networkObject">The network object to despawn.</param>
    /// <param name="_prefabRef">The prefab reference of the network object.</param>
    public void InstantDespawn(NetworkObject _networkObject, GameObject _prefabRef)
    {
        NetworkObjectPool.Singleton.ReturnNetworkObject(_networkObject, _prefabRef);

        if (!_networkObject.IsSpawned) return;

        ObjectProperties objectProperties = _networkObject.GetComponent<ObjectProperties>();
        NetworkWavesController.Singleton.ObstacleAmountStatusTracker(objectProperties.typ, false);
        objectProperties.OnDespawn();
        _networkObject.Despawn(false);
    }
}