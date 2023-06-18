using UnityEngine;

public class WorldSizeController : MonoBehaviour
{
    public static readonly Vector2 worldDimensionsMin = new(-23, -16);
    public static readonly Vector2 worldDimensionsMax = new(10, 17);

    public static bool IsObjectOutsideWorldDimensions(Vector2 _position)
    {
        return _position.x <= worldDimensionsMin.x - 5 ||
               _position.x >= worldDimensionsMax.x + 5 ||
               _position.y <= worldDimensionsMin.y - 5 ||
               _position.y >= worldDimensionsMax.y + 5;
    }
}