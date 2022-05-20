using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null;
    public SaveState save = null;
    public SaveState save1, save2, save3;
    public int saveNum = 0;
    
    //Level info
    public int currentLevel;
    //public levelDatabase levelDatabase;
    //public List<Level> completedLevels = new List<Level>();

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

        //Update Save Menu UI if save slot was created before
        UpdateSaveMenuUI();
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
        save.currentLevel = 0;
        mapPosition = Vector2Int.zero;
        save.Collection.Clear();
        save.Save();
        UpdateSaveMenuUI();
    }

    public void UpdateSaveMenuUI()
    {
        if (save1.saveCreated)
        {
            GameObject.Find("save1Button").GetComponentInChildren<Text>().text = "Save Slot 1";
            GameObject.Find("deleteSave1").GetComponent<Image>().enabled = true;
        }
        else
        {
            GameObject.Find("save1Button").GetComponentInChildren<Text>().text = "New Save";
            GameObject.Find("deleteSave1").GetComponent<Image>().enabled = false;

        }

        if (save2.saveCreated)
        {
            GameObject.Find("save2Button").GetComponentInChildren<Text>().text = "Save Slot 2";
            GameObject.Find("deleteSave2").GetComponent<Image>().enabled = true;
        }
        else
        {
            GameObject.Find("save2Button").GetComponentInChildren<Text>().text = "New Save";
            GameObject.Find("deleteSave2").GetComponent<Image>().enabled = false;
        }

        if (save3.saveCreated)
        {
            GameObject.Find("save3Button").GetComponentInChildren<Text>().text = "Save Slot 3";
            GameObject.Find("deleteSave3").GetComponent<Image>().enabled = true;
        }
        else
        {
            GameObject.Find("save3Button").GetComponentInChildren<Text>().text = "New Save";
            GameObject.Find("deleteSave3").GetComponent<Image>().enabled = false;
        }
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
                save.currentLevel++;
            }
        }
    }
}
