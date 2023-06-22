using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public MovementController movementController;
    public ShootingController shootingController;
    private CameraController cameraController;
    public PlayerHealthController playerHealthController;
    private GameObject camera;
    public ulong playerID;

    public bool blockInput = false;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        shootingController = GetComponent<ShootingController>();
        playerHealthController = GetComponent<PlayerHealthController>();
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

        movementController.HandleMovementInput(blockInput);
        shootingController.HandleShootingInput(blockInput);

        if (blockInput) return;

        if (camera.activeSelf && cameraController != null)
            cameraController.HandleCameraMovement();

        if (Input.GetKeyDown(KeyCode.B))
            ShopController.instance.ToggleShop();
    }
}