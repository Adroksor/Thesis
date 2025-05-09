using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Scene names")]
    [SerializeField] string newGameScene = "Game";   // gameplay scene name

    [SerializeField] Button newBtn, loadBtn, quitBtn;
    [SerializeField] string gameScene = "Game";

    void Awake()
    {
        newBtn .onClick.AddListener(OnNewGame);

        loadBtn.onClick.AddListener(OnLoadGame);

        quitBtn.onClick.AddListener(OnQuit);
    }
    
    
    public void OnNewGame()
    {
        // optional: clear save data here
        GameBootstrap.Instance.mode = LaunchMode.NewGame;
        SceneManager.LoadScene(newGameScene);
    }

    public void OnLoadGame()
    {
        GameBootstrap.Instance.mode = LaunchMode.LoadGame;
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
