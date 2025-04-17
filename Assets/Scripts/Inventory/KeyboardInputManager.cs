using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyboardInputManager : MonoBehaviour
{
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