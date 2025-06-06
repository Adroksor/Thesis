using UnityEngine;
using UnityEngine.SceneManagement;

public enum LaunchMode { None, NewGame, LoadGame }

public class GameBootstrap : MonoBehaviour
{
    public static GameBootstrap Instance { get; private set; }

    public LaunchMode   mode       = LaunchMode.None;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode lsm)
    {
        if (scene.name != "Game") return;


        switch (mode)
        {
            case LaunchMode.NewGame:
                WorldGenerator.instance.SetSeed(Random.Range(100000, 999999));
                break;

            case LaunchMode.LoadGame:
                WorldGenerator.instance.SetSeed(SaveSystem.GetSeed());
                SaveSystem.Load(); // builds pristine world
                break;
        }
    }
}