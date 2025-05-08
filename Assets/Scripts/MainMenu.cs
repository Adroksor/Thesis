using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene names")]
    [SerializeField] string newGameScene = "Game";   // gameplay scene name

    public void OnNewGame()
    {
        // optional: clear save data here
        SceneManager.LoadScene(newGameScene);
    }

    public void OnLoadGame()
    {
        SceneManager.LoadScene(newGameScene);
    }

    public void OnQuit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
