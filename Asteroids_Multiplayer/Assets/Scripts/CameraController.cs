using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetPos;

    private Vector2 minBoundaries = new(WorldSizeController.worldDimensionsMin.x + 10,
        WorldSizeController.worldDimensionsMin.y + 5);

    private Vector2 maxBoundaries = new(WorldSizeController.worldDimensionsMax.x - 10,
        WorldSizeController.worldDimensionsMax.y - 5);

    public void HandleCameraMovement()
    {
        Vector3 currentPosition = new(targetPos.position.x, targetPos.position.y, transform.position.z);

        float clampedX = Mathf.Clamp(currentPosition.x, minBoundaries.x, maxBoundaries.x);
        float clampedY = Mathf.Clamp(currentPosition.y, minBoundaries.y, maxBoundaries.y);
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, currentPosition.z);

        if (IsOutsideBounds(clampedPosition))
            transform.position = clampedPosition;
        else
            transform.position = new(targetPos.position.x, targetPos.position.y, currentPosition.z);

        transform.rotation = quaternion.identity;
    }

    private bool IsOutsideBounds(Vector3 position)
    {
        return position.x <= minBoundaries.x ||
               position.x >= maxBoundaries.x ||
               position.y <= minBoundaries.y ||
               position.y >= maxBoundaries.y;
    }
}