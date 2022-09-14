using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UI_script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void play_game()
    {
        SceneManager.LoadScene("Save Menu");
    }

    public void quit_game()
    {
        Application.Quit();
    }

    // UNUSED
    public void openCollection()
    {
        SceneManager.LoadScene("Collection");
    }

    public void backToMain()
    {
        //SaveManager.instance.ResetSave();
        SceneManager.LoadScene("Main Menu");
    }

    public void backToSave()
    {
        SaveManager.instance.Save();
        SceneManager.LoadScene("Save Menu");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    // UNUSED
    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    // UNUSED
    public void LoadControls()
    {
        SceneManager.LoadScene("Controls");
    }

    public void LoadExtras()
    {
        SceneManager.LoadScene("Extras");
    }

    public void LoadSave(int saveNum)
    {
        if (GameObject.Find("save" + saveNum + "Button").GetComponentInChildren<TextMeshProUGUI>().text == "New Save")
        {
            SaveManager.instance.NewSave(saveNum);
        }
        else
        {
            SaveManager.instance.LoadSave(saveNum);
            SceneManager.LoadScene("world_map");
        }
    }

    public void DeleteSave(int saveNum)
    {
        SaveManager.instance.ClearSave(saveNum);
    }
}
