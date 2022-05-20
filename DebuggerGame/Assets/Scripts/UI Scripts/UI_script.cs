using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene("world_map");
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
        SaveManager.instance.LoadSave(saveNum);
        System.Console.Write(saveNum);
        SceneManager.LoadScene("collection");
    }
}
