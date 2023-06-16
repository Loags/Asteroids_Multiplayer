using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class ShootingController : NetworkBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
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

        Projectile projectile = spawnedNetworkObject.gameObject.GetComponent<Projectile>();
        projectile.Launch(transform.rotation);

        await NetworkObjectSpawner.Singleton.WaitThenDespawn(spawnedNetworkObject, projectilePrefab,
            projectile.lifeSpan);
    }
}