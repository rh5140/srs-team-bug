using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool MapIsOpen = false;
    public GameObject PauseMenuUI;
    public GameObject WorldMap;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            if(MapIsOpen)
                CloseMap();
            else
                OpenMap();
        }
    }
    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        GameIsPaused = false;
    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        GameIsPaused = true;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("level 1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void openCollection()
    {
        SceneManager.LoadScene("collection");
    }

    public void LoadMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("main_menu");
    }

    public void OpenMap()
    {
        WorldMap.SetActive(true);
        MapIsOpen = true;
    }

    public void CloseMap()
    {
        WorldMap.SetActive(false);
        MapIsOpen = false;
    }
}
