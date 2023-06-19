using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private MovementController movementController;
    private ShootingController shootingController;
    private CameraController cameraController;
    private GameObject camera;
    public ulong playerID;


    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        shootingController = GetComponent<ShootingController>();
        camera = transform.GetChild(2).gameObject;
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("Player ID: " + gameObject.GetComponent<NetworkObject>().OwnerClientId);
        playerID = gameObject.GetComponent<NetworkObject>().OwnerClientId;

        if (!GetComponent<NetworkObject>().IsOwner) return;
        camera.SetActive(true);
        cameraController = GetComponentInChildren<CameraController>();
    }

    private void Update()
    {
        if (!GetComponent<NetworkObject>().IsOwner) return;

        movementController.HandleMovementInput();
        shootingController.HandleShootingInput();
        if (camera.activeSelf && cameraController != null)
            cameraController.HandleCameraMovement();
    }
}