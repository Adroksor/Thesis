using UnityEngine;

public class TopDownPlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float walkSpeed = 5f; // Speed of the player movement
    public float sprintSpeed = 20f;
    public Rigidbody2D rb; // Reference to the Rigidbody2D component
    public WorldGenerator worldGenerator; // Reference to the WorldGenerator script

    private Vector2 movement; // Stores the player's movement input

    void Update()
    {
        // Get input from the player
        movement.x = Input.GetAxisRaw("Horizontal"); // Left/Right or A/D
        movement.y = Input.GetAxisRaw("Vertical"); // Up/Down or W/S

        // Normalize the movement vector to prevent faster diagonal movement
        movement = movement.normalized;

        // Check for reset input
        if (Input.GetKeyDown(KeyCode.R)) // Press R to reset the world
        {
            ResetWorld();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = sprintSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }
    }

    void FixedUpdate()
    {
        // Move the player using Rigidbody2D
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void ResetWorld()
    {
        if (worldGenerator != null)
        {
            // Reset the world by clearing existing chunks and regenerating
            worldGenerator.ResetWorld();
            Debug.Log("World has been reset!");
        }
        else
        {
            Debug.LogWarning("WorldGenerator reference is missing!");
        }
    }
}