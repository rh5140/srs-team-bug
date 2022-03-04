using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null;
    public SaveState save = null;

    void Awake()
    {
        //Singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void setSave(int slot)
    {
        save = Resources.Load<SaveState>("Save State {slot}");
    }

    public void ResetSave()
    {
        save = null;
    }

    public void loadLevel()
    {
        //
    }
}
