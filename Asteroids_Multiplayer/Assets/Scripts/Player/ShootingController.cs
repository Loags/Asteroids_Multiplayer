using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class ShootingController : NetworkBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float shootingCooldown;

    public void HandleShootingInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            InvokeRepeating(nameof(SpawnProjectileServerRpc), 0f, shootingCooldown);
        else if (Input.GetKeyUp(KeyCode.Space))
            CancelInvoke(nameof(SpawnProjectileServerRpc));
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnProjectileServerRpc()
    {
        GameObject projectileGO =
            Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        projectileGO.GetComponent<NetworkObject>().Spawn();

        Projectile projectile = projectileGO.GetComponent<Projectile>();
        projectile.Launch(transform.rotation);
    }
}