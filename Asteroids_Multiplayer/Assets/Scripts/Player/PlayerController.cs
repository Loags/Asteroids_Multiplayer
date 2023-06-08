using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementController movementController;
    private ShootingController shootingController;
    private PlayerData playerData = new();

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        shootingController = GetComponent<ShootingController>();
        playerData.SetPlayerData(NetworkManager.Singleton.LocalClientId);
    }

    private void Update()
    {
        if (!GetComponent<NetworkObject>().IsOwner)
        {
            Debug.Log("Not the Owner cant move");
            Destroy(this);
            return;
        }

        Debug.Log("Is Owner can move");

        movementController.HandleMovementInput();
        shootingController.HandleShootingInput();
    }
}