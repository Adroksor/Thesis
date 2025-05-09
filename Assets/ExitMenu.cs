using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitMenu : MonoBehaviour
{
    
    [Header("Scene names")]
    [SerializeField] string mainMenuScene = "MainMenu";   // gameplay scene name

    public void SaveGame()
    {
        SaveSystem.Save();
    }

    public void ReturnToGame()
    {
        gameObject.SetActive(false);
    }
    public void OnQuit()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
