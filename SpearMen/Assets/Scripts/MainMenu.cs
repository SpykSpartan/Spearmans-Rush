using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayNewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void PlayShitGame()
    {
        SceneManager.LoadScene("ShittyScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    } 

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
