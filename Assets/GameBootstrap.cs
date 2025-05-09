using UnityEngine;
using UnityEngine.SceneManagement;

public enum LaunchMode { None, NewGame, LoadGame }

public class GameBootstrap : MonoBehaviour
{
    public static GameBootstrap Instance { get; private set; }

    public LaunchMode   mode       = LaunchMode.None; // set by MainMenu
    public SaveSystem.SaveData    pendingSave;                  // filled for LoadGame

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);

        // When a new scene loads, decide what to do
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    /* ----------------------------------------------------------------- */
    void OnSceneLoaded(Scene scene, LoadSceneMode lsm)
    {
        if (scene.name != "Game") return;          // ignore Menu scene loads


        switch (mode)
        {
            case LaunchMode.NewGame:
                WorldGenerator.instance.SetSeed(Random.Range(100000, 999999)); // or from menu
                WorldGenerator.instance.GenerateInitialChunks();                           // your Start()
                break;

            case LaunchMode.LoadGame:
                WorldGenerator.instance.GenerateInitialChunks();                           // your Start()
                SaveSystem.Load(); // builds pristine world
                break;
        }
    }
}