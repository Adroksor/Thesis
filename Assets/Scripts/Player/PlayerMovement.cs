using Unity.VisualScripting;
using UnityEngine;

public class TopDownPlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float walkSpeed = 5f; // Speed of the player movement
    public float sprintSpeed = 20f;
    
    public Animator animator;
    
    
    public Rigidbody2D rb; // Reference to the Rigidbody2D component
    public WorldGenerator worldGenerator; // Reference to the WorldGenerator script

    private Vector2 movement; // Stores the player's movement input

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        

        movement = movement.normalized;


        bool isMoving = movement.sqrMagnitude > 0.01f;
        animator.SetBool("isMoving", isMoving);

        GameManager.instance.playerPosition = transform.position;
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetWorld();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadWorldFromSeed();
        }
        
        if(Input.GetKeyDown(KeyCode.Slash))
        {
            SaveManager.instance.SaveChunks();
        }
        
        if(Input.GetKeyDown(KeyCode.Keypad7))
        {
            SaveSystem.Save();
        }
        
        if(Input.GetKeyDown(KeyCode.Keypad9))
        {
            SaveSystem.Load();
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
        rb.velocity = movement * moveSpeed; 
    }

    void ResetWorld()
    {
        if (worldGenerator != null)
        {
            // Reset the world by clearing existing chunks and regenerating
            GameManager.instance.seed = Random.Range(100000, 999999);
            
            worldGenerator.ResetWorld();
            Debug.Log("World has been reset!");
        }
        else
        {
            Debug.LogWarning("WorldGenerator reference is missing!");
        }
    }

    void LoadWorldFromSeed()
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
