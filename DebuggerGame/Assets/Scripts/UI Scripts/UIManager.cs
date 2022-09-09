/* NOTE:
 * THIS SCRIPT IS (MOST LIKELY) COMPLETELY UNUSED
 * I HAVEN'T CHECKED EVERYTHING TO MAKE COMPLETE SURE THOUGH SO I'M LEAVING IT IN - RAY
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    private bool paused_;
    public bool paused
    {
        get => paused_;
        set
        {
            if(paused_ != value)
            {
                paused_ = value;
                if (value)
                {
                    Board.instance.PushGameState(Board.GameState.Paused);
                    pauseMenu.alpha = 1;
                    pauseMenu.interactable = true;
                    eventSystem.SetSelectedGameObject(pauseMenuFirstSelected);
                }
                else
                {
                    Board.instance.PopGameState();
                    pauseMenu.alpha = 0;
                    pauseMenu.interactable = false;
                }
            }
            
        }
    }

    [System.NonSerialized]
    public bool MapIsOpen = false;
    [System.NonSerialized]
    public bool InventoryIsOpen = false;

    public EventSystem eventSystem;

    public CanvasGroup pauseMenu;
    public GameObject controls;
    public GameObject inventory;

    public GameObject pauseMenuFirstSelected;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        pauseMenu.alpha = 0;
        pauseMenu.interactable = false;
        controls.SetActive(false);
        inventory.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
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

    public void LoadSave()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Save Menu");
    }

    public void OpenMap()
    {
        controls.SetActive(true);
        MapIsOpen = true;
    }

    public void CloseMap()
    {
        controls.SetActive(false);
        MapIsOpen = false;
    }

    public void OpenInventory()
    {
        inventory.SetActive(true);
        InventoryIsOpen = true;
    }

    public void CloseInventory()
    {
        inventory.SetActive(false);
        InventoryIsOpen = false;
    }
}
