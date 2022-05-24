using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("Scene is Loaded");
        //Debug.Log(save1);
        //Debug.Log(save2);
        //Debug.Log(save3);
        if (scene.name == "Save Menu")
        {
            UpdateSaveMenuUI();
        }
        
    }

    private void Start()
    {
        
        //save = (SaveState)Resources.Load<SaveState>("Assets / Resources / Save State.asset");
        //currentLevel = save.currentLevel;
        //mapPosition = save.mapPosition;
        //Collection = save.Collection;
    }

    public void NewSave(int saveNum)
    {
        if (saveNum == 1)
            save = save1;
        else if (saveNum == 2)
            save = save2;
        else if (saveNum == 3)
            save = save3;

        save.saveCreated = true;
        save.currentLevel = "0000";
        currentLevel = "0000";
        save.unlockedLevels.Add("0000");
        unlockedLevels.Add("0000");
        Save();
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
        // mapPosition = save.mapPosition;
        unlockedLevels = save.unlockedLevels;
        Collection = save.Collection;
    }

    public void Save()
    {
        save.Save();
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
        UpdateSaveMenuUI();

    }

    public void UpdateSaveMenuUI()
    {
        if (save1.saveCreated)
        {
            GameObject.Find("save1Button").GetComponentInChildren<TextMeshProUGUI>().text = "Save Slot 1";
            GameObject.Find("deleteSave1").GetComponent<Image>().enabled = true;
        }
        else
        {
            GameObject.Find("save1Button").GetComponentInChildren<TextMeshProUGUI>().text = "New Save";
            GameObject.Find("deleteSave1").GetComponent<Image>().enabled = false;

        }

        if (save2.saveCreated)
        {
            GameObject.Find("save2Button").GetComponentInChildren<TextMeshProUGUI>().text = "Save Slot 2";
            GameObject.Find("deleteSave2").GetComponent<Image>().enabled = true;
        }
        else
        {
            GameObject.Find("save2Button").GetComponentInChildren<TextMeshProUGUI>().text = "New Save";
            GameObject.Find("deleteSave2").GetComponent<Image>().enabled = false;
        }

        if (save3.saveCreated)
        {
            GameObject.Find("save3Button").GetComponentInChildren<TextMeshProUGUI>().text = "Save Slot 3";
            GameObject.Find("deleteSave3").GetComponent<Image>().enabled = true;
        }
        else
        {
            GameObject.Find("save3Button").GetComponentInChildren<TextMeshProUGUI>().text = "New Save";
            GameObject.Find("deleteSave3").GetComponent<Image>().enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            foreach (var value in unlockedLevels)
            {
                Debug.Log(value);
            }
        }
    }

}
