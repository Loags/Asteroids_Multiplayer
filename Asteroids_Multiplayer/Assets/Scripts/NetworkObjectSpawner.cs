using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class NetworkObjectSpawner : NetworkBehaviour
{
    public static NetworkObjectSpawner Singleton;

    // Used to cancel specific tasks, when InstantDespawn is being called
    private Dictionary<NetworkObject, CancellationTokenSource> cancellationTokenSources =
        new Dictionary<NetworkObject, CancellationTokenSource>();
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

    public async Task WaitThenDespawn(NetworkObject _networkObject, GameObject _prefabRef, float _lifeSpan)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        // Store the cancellation token source for later reference
        cancellationTokenSources[_networkObject] = cancellationTokenSource;

        try
        {
            int delay = (int)(_lifeSpan * 1000);
            await Task.Delay(delay, cancellationToken);
            Despawn(_networkObject, _prefabRef);
        }
        catch (TaskCanceledException)
        {
            // The task was canceled, do not despawn the object
        }
        finally
        {
            // Remove the stored cancellation token source
            cancellationTokenSources.Remove(_networkObject);
        }
    }

    public void InstantDespawn(NetworkObject _networkObject, GameObject _prefabRef)
    {
        if (cancellationTokenSources.TryGetValue(_networkObject, out CancellationTokenSource cancellationTokenSource))
        {
            // Cancel the associated task if it exists
            cancellationTokenSource.Cancel();
        }
        
        Despawn(_networkObject, _prefabRef);
    }
    private void Despawn(NetworkObject _networkObject, GameObject _prefabRef)
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