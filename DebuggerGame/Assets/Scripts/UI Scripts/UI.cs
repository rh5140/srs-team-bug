using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool ControlIsOpen = false;
    public static bool OptionIsOpen = false;

    public static bool InventoryIsOpen = false;
    public GameObject OptionsMenu;
    public GameObject ControlScreen;
    public GameObject Inventory;
    public GameObject LevelName;

    void Start()
    {
        string levelName = Board.instance.levelName;
        levelName = "Level " + levelName.Substring(0,2) + "-" + levelName.Substring(2, 2);
        LevelName.GetComponent<TMPro.TextMeshProUGUI>().SetText(levelName);

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {

            if (GameIsPaused)
            {
                CloseAll();
                Resume();
            }
            else
            {
                OpenOptions();
            }

        }

        // CONTROLS, NOT WORLD MAP ANYMORE
        if(Input.GetKeyDown(KeyCode.C))
        {
            if (ControlIsOpen)
            {
                CloseAll();
                Resume();
            }  
            else
            {
                OpenControls();
            }

        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (InventoryIsOpen)
            {
                CloseAll();
                Resume();
            }  
            else
            {
                OpenInventory();
            }

        }
    }

    public void Exit()
    {
        CloseAll();
        Resume();
    }

    public void CloseAll()
    {
        CloseInventory();
        CloseControls();
        CloseOptions();
    }

    public void Resume()
    {
        Time.timeScale = 1;
        GameIsPaused = false;
    }

    public void Pause()
    {
        Time.timeScale = 0;
        GameIsPaused = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    /* ?
    public void openCollection()
    {
        SceneManager.LoadScene("collection");
    }
    */

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

    public void LoadSave()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Save Menu");
    }



    public void OpenOptions()
    {
        CloseAll();
        Pause();
        OptionsMenu.SetActive(true);
    }

    public void CloseOptions()
    {
        OptionsMenu.SetActive(false);
    }

    public void OpenControls()
    {
        CloseAll();
        //Pause(); if you pause, the control screen popout animation cannot play
        ControlScreen.SetActive(true);
        ControlIsOpen = true;
    }

    public void CloseControls()
    {
        ControlScreen.SetActive(false);
        ControlIsOpen = false;
    }

    public void OpenInventory()
    {
        CloseAll();
        Pause();
        Inventory.SetActive(true);
        InventoryIsOpen = true;
    }

    public void CloseInventory()
    {
        Inventory.SetActive(false);
        InventoryIsOpen = false;
    }
}
