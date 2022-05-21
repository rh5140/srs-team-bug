using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool MapIsOpen = false;

    public static bool InventoryIsOpen = false;
    public GameObject PauseMenuUI;
    public GameObject WorldMap;
    public GameObject Inventory;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }

        // CONTROLS, NOT WORLD MAP ANYMORE
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(MapIsOpen)
                CloseMap();
            else
                OpenMap();
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(InventoryIsOpen)
                CloseInventory();
            else
                OpenInventory();
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
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadMap()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("world_map");
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

    public void OpenInventory()
    {
        Inventory.SetActive(true);
        InventoryIsOpen = true;
    }

    public void CloseInventory()
    {
        Inventory.SetActive(false);
        InventoryIsOpen = false;
    }
}
