using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementController movementController;
    private ShootingController shootingController;
    private PlayerData playerData;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        shootingController = GetComponent<ShootingController>();
        playerData.SetPlayerData(NetworkManager.Singleton.LocalClientId);
    }

    private void Update()
    {
        if (!GetComponent<NetworkObject>().IsOwner) return;

        movementController.HandleMovementInput();
        shootingController.HandleShootingInput();
    }
}