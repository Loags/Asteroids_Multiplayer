using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementController movementController;
    private ShootingController shootingController;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        shootingController = GetComponent<ShootingController>();
    }

    private void Update()
    {
        if (!GetComponent<NetworkObject>().IsOwner) return;

        movementController.HandleMovementInput();
        shootingController.HandleShootingInput();
    }
}