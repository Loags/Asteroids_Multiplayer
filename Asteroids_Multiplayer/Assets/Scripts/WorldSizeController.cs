using UnityEngine;

public class WorldSizeController : MonoBehaviour
{
    public static readonly Vector2 worldDimensionsMin = new(-23, -16);
    public static readonly Vector2 worldDimensionsMax = new(10, 17);

    private static readonly float despawnOffSet = 10;
    private static readonly float spawnOffSet = 5;

    public static bool IsObjectOutsideWorldDimensions(Vector2 _position)
    {
        return _position.x <= worldDimensionsMin.x - despawnOffSet ||
               _position.x >= worldDimensionsMax.x + despawnOffSet ||
               _position.y <= worldDimensionsMin.y - despawnOffSet ||
               _position.y >= worldDimensionsMax.y + despawnOffSet;
    }

    public static Vector3 GenerateSpawnPoint()
    {
        int randomIndex = Random.Range(0, 4);
        Vector3 spawnPoint = Vector3.zero;

        switch (randomIndex)
        {
            case 0:
                spawnPoint.x = worldDimensionsMin.x - spawnOffSet;
                spawnPoint.y = Random.Range(worldDimensionsMin.y, worldDimensionsMax.y);
                break;
            case 1:
                spawnPoint.x = worldDimensionsMax.x + spawnOffSet;
                spawnPoint.y = Random.Range(worldDimensionsMin.y, worldDimensionsMax.y);
                break;
            case 2:
                spawnPoint.x = Random.Range(worldDimensionsMin.x, worldDimensionsMax.x);
                spawnPoint.y = worldDimensionsMin.y - spawnOffSet;
                break;
            case 3:
                spawnPoint.x = Random.Range(worldDimensionsMin.x, worldDimensionsMax.x);
                spawnPoint.y = worldDimensionsMax.y + spawnOffSet;
                break;
        }

        return spawnPoint;
    }
}