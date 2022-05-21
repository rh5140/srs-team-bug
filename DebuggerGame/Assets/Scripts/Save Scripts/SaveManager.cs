using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null;
    public SaveState save;
    public SaveState save1, save2, save3;
    public int saveNum = 0;
    
    //Level info
    public string currentLevel;
    //public levelDatabase levelDatabase;
    //public List<Level> completedLevels = new List<Level>();
    public HashSet<string> unlockedLevels = new HashSet<string>();

    //Map info
    public Vector2Int mapPosition;

    //Collection info
    public ArthropodDatabase arthropodDatabase;
    public List<InventorySlot> Collection = new List<InventorySlot>();

    private void Awake()
    {
        //Singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //save = (SaveState)Resources.Load<SaveState>("Assets / Resources / Save State.asset");
        //currentLevel = save.currentLevel;
        //mapPosition = save.mapPosition;
        //Collection = save.Collection;
    }

    public void ResetSave()
    {
        if (saveNum == 1)
            save1 = save;
        else if (saveNum == 2)
            save2 = save;
        else if (saveNum == 3)
            save = save3;
        save.currentLevel = null;
        save.unlockedLevels = new HashSet<string>();
        save.saveCreated = true;
        UpdateSaveMenuUI();
    }

    public void LoadSave(int saveNum)
    {
        if (saveNum == 1)
            save = save1;
        else if (saveNum == 2)
            save = save2;
        else if (saveNum == 3)
            save = save3;
        this.saveNum = saveNum;
        save.Load();
        currentLevel = save.currentLevel;
        mapPosition = save.mapPosition;
        unlockedLevels = save.unlockedLevels;
    }

    public void ClearSave(int saveNum)
    {
        if (saveNum == 1)
            save = save1;
        else if (saveNum == 2)
            save = save2;
        else if (saveNum == 3)
            save = save3;

        save.saveCreated = false;
        save.currentLevel = null;
        save.unlockedLevels = new HashSet<string>();
        mapPosition = Vector2Int.zero;
        save.Collection.Clear();
        save.Save();

    }

    public void LoadLevel()
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

            if (Input.GetKeyDown(KeyCode.Z))
            {
                //save.currentLevel++;
                Debug.Log("Unexpected behavior: Check the code and ask Felix Peng");
            }
        }
    }

    public void LoadSave(int saveNum)
    {
        if (saveNum == 1)
            save = save1;
        else if (saveNum == 2)
            save = save2;
        else if (saveNum == 3)
            save = save3;
        this.saveNum = saveNum;
        save.Load();
        currentLevel = save.currentLevel;
        mapPosition = save.mapPosition;
        Collection = save.Collection;
    }
}
