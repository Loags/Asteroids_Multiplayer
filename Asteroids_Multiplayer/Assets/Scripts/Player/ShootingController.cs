using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class ShootingController : NetworkBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Transform projectileEffectSpawnPoint;
    [SerializeField] private float shootingCooldown;

    public void HandleShootingInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            InvokeRepeating(nameof(ShootServerRpc), 0f, shootingCooldown);
        else if (Input.GetKeyUp(KeyCode.Space))
            CancelInvoke(nameof(ShootServerRpc));
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        Shoot();
    }

    private async void Shoot()
    {
        NetworkObject spawnedNetworkObject = NetworkObjectSpawner.SpawnObjectByTypAtPosition(projectilePrefab,
            projectileSpawnPoint.position,
            transform.rotation);

        ObjectProjectile objectProjectile = spawnedNetworkObject.gameObject.GetComponent<ObjectProjectile>();
        objectProjectile.Launch(transform.rotation);

        await NetworkObjectSpawner.Singleton.WaitThenDespawn(spawnedNetworkObject, projectilePrefab,
            objectProjectile.lifeSpan);
    }
}