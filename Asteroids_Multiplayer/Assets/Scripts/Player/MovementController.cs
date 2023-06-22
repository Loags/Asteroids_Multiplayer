using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void HandleMovementInput(bool _blockInput = false)
    {
        if (_blockInput)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        rb.velocity = movement * moveSpeed;

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, WorldSizeController.worldDimensionsMin.x + 2,
            WorldSizeController.worldDimensionsMax.x - 2);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, WorldSizeController.worldDimensionsMin.y + 2,
            WorldSizeController.worldDimensionsMax.y - 2);
        transform.position = clampedPosition;

        RotateTowardsMovement(movement);
    }

    private void RotateTowardsMovement(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            targetAngle -= 90f;

            Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void IncreaseRotationSpeed(float _value) => rotationSpeed += _value;
    public void IncreaseMoveSpeed(float _value) => moveSpeed += _value;
}