using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Normal walking speed.")]
    public float moveSpeed = 5.0f;

    [Tooltip("Multiplier applied when holding Left Shift.")]
    public float sprintMultiplier = 2.0f;

    void Update()
    {
        // 1. Get raw input from WASD or Arrow Keys (-1 to 1)
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S

        // 2. Check if the user is holding Left Shift to sprint
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintMultiplier;
        }

        // 3. Calculate the movement direction relative to the camera's local rotation
        Vector3 direction = (transform.right * horizontal) + (transform.forward * vertical);

        // 4. Apply the translation, scaling by Time.deltaTime for smooth, framerate-independent movement
        transform.position += direction * currentSpeed * Time.deltaTime;

        // Optional: Add simple vertical movement with Q and E
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += Vector3.up * currentSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += Vector3.down * currentSpeed * Time.deltaTime;
        }
    }
}