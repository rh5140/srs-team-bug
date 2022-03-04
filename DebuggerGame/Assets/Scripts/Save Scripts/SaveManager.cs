using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null;
    public SaveState save;

    void Awake()
    {
        //Singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        save = Resources.Load<SaveState>("Save State 1");
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

    private void Update()
    {
        if (save != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                save.Save();
                Debug.Log("save");
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                save.Load();
                Debug.Log("load");
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                save.AddArthropod(save.arthropodDatabase.GetArthropod[0], 1);
                Debug.Log("ant");
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                save.AddArthropod(save.arthropodDatabase.GetArthropod[1], 1);
                Debug.Log("spider");
            }
        }
    }
}
