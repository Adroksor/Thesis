using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyboardInputManager : MonoBehaviour
{
    public ExitMenu exitMenu;
    void Update()
    {
        // Reload current scene
        if (Input.GetKeyDown(KeyCode.M))
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

        // Quit application
        if (Input.GetKeyDown(KeyCode.P))
        {
            QuitApplication();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            int interactableLayer = LayerMask.GetMask("Interactable");
            
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, math.INFINITY, interactableLayer);
            if (hit.collider != null)
            {
                var chest = hit.collider.GetComponentInParent<Chest>();
                if (chest != null)
                {
                    if (InventoryManager.instance.currentlyInteractedObject == null)
                    {
                        chest.OpenInventory();
                    }
                }
                
                var furnace = hit.collider.GetComponentInParent<Furnace>();
                if (furnace != null)
                {
                    if (InventoryManager.instance.currentlyInteractedObject == null)
                    {
                        furnace.OpenFurnace();
                    }
                }
                
                var workbench = hit.collider.GetComponentInParent<Workbench>();
                if (workbench != null)
                {
                    {
                        if (InventoryManager.instance.currentlyInteractedObject == null)
                        {
                            workbench.OpenWorkbench();
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitMenu.gameObject.SetActive(true);
        }
    }
    
    

    private void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // Stops play mode in editor
#else
        Application.Quit(); // Quits the build
#endif
    }
}