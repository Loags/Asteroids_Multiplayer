using Unity.Netcode;
using UnityEngine;

public class ShootingController : NetworkBehaviour
{
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float shootingCooldown;
    [SerializeField] private float damage;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void HandleShootingInput(bool _blockInput = false)
    {
        if (_blockInput)
        {
            CancelInvoke(nameof(LocalShootServerRpc));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            InvokeRepeating(nameof(LocalShootServerRpc), 0f, shootingCooldown);
        else if (Input.GetKeyUp(KeyCode.Space))
            CancelInvoke(nameof(LocalShootServerRpc));
    }

    public void IncreaseDamage(int _value) => damage += _value;

    public void IncreaseShootingSpeed(float _value) => shootingCooldown -= _value;


    [ServerRpc(RequireOwnership = false)]
    private void LocalShootServerRpc()
    {
        LocalShootClientRpc();
    }

    [ClientRpc]
    private void LocalShootClientRpc()
    {
        LocalShoot();
    }

    private void LocalShoot()
    {
        GameObject spawnedProjectile = ObjectPool.instance.GetObjectFromPool();
        spawnedProjectile.transform.position = projectileSpawnPoint.position;
        spawnedProjectile.transform.rotation = transform.rotation;

        ObjectProjectile objectProjectile = spawnedProjectile.GetComponent<ObjectProjectile>();
        if (PlayerDataManager.instance.damageMultiplierActive)
        {
            int modifiedDamage = (int)(damage * PlayerDataManager.instance.damageMultiplier);
            objectProjectile.LocalLaunch(transform.rotation, playerController.playerID, modifiedDamage);
        }
        else
            objectProjectile.LocalLaunch(transform.rotation, playerController.playerID, damage);
    }
}