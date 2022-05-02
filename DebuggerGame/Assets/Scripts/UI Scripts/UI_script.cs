using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        SceneManager.LoadScene("save menu");
    }

    public void quit_game()
    {
        Application.Quit();
    }

    public void openCollection()
    {
        SceneManager.LoadScene("collection");
    }

    public void backToMain()
    {
        //SaveManager.instance.ResetSave();
        SceneManager.LoadScene("save menu");
    }

    public void LoadSave(int saveNum)
    {
        if (GameObject.Find("save" + saveNum + "Button").GetComponentInChildren<Text>().text == "New Save")
        {
            SaveManager.instance.NewSave(saveNum);
        }
        else
        {
            SaveManager.instance.LoadSave(saveNum);
            Debug.Log(saveNum);
            SceneManager.LoadScene("world_map");
        }
    }

    public void DeleteSave(int saveNum)
    {
        SaveManager.instance.ClearSave(saveNum);
    }
}
