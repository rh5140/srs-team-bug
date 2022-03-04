using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null;
    public SaveSystem saveState;

    void Awake()
    {
        //Singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void setSave(SaveSystem save)
    {
        
    }

    public void ResetSave()
    {
        saveState = null;
    }

    public void loadLevel()
    {

    }
}
