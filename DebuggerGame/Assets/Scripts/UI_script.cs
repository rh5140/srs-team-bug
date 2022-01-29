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
        SceneManager.LoadScene("level 1");
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
        SceneManager.LoadScene("main_menu");
    }
}
