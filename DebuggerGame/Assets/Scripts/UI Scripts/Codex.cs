using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Codex : MonoBehaviour
{
    public Dictionary<string, GameObject> characterEntries = new Dictionary<string, GameObject>();

    public static bool codexOpen = false;
    public GameObject codexDisplay;
    public GameObject denizensDisplay;
    public GameObject bugsDisplay;

    //Denizen entries
    private GameObject elderFlytrapEntry;
    private GameObject cryingCloverEntry;
    private GameObject flyKingEntry;

    //Enemy entries
    private GameObject basicFlyEntry;
    private GameObject weirdFlyEntry;

    // Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    void Start() 
    {
        //Find denizen entries
        elderFlytrapEntry = denizensDisplay.transform.Find("Scroll View/Viewport/Content/Elder Flytrap Entry").gameObject;
        cryingCloverEntry = denizensDisplay.transform.Find("Scroll View/Viewport/Content/Crying Clover Entry").gameObject;
        flyKingEntry = denizensDisplay.transform.Find("Scroll View/Viewport/Content/Fly King Entry").gameObject;

        //Find enemy entries
        basicFlyEntry = bugsDisplay.transform.Find("Scroll View/Viewport/Content/Basic Fly Entry").gameObject;
        weirdFlyEntry = bugsDisplay.transform.Find("Scroll View/Viewport/Content/Weird Fly Entry").gameObject;

        string[] characterNames = {"elderFlytrap", "cryingClover1", "cryingClover2", "flyKing", "basicFly", "weirdFly"};

        characterEntries.Add("elderFlytrap", elderFlytrapEntry);
        characterEntries.Add("cryingClover1", cryingCloverEntry);
        characterEntries.Add("cryingClover2", cryingCloverEntry);
        characterEntries.Add("flyKing", flyKingEntry);
        characterEntries.Add("basicFly", basicFlyEntry);
        characterEntries.Add("weirdFly", weirdFlyEntry);

        foreach (string characterName in characterNames)
        {
            if (!SaveManager.instance.unlockedCharacters.Contains(characterName))
            {
                HideEntry(characterEntries[characterName], characterName);
            }
        }

        foreach (string newUnlock in SaveManager.instance.newestCharacterUnlocks) 
        {
            characterEntries[newUnlock].transform.Find("Image").gameObject.GetComponent<Image>().color = Color.red;
        }
    }

    // Functionality methods

    public void HideEntry(GameObject characterEntry, string name)
    {
        switch (name) 
        {
            case "cryingClover1":
                characterEntry.transform.Find("Image").gameObject.GetComponent<Image>().color = Color.black;
                characterEntry.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = "???";
                characterEntry.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "???";
                break;

            case "cryingClover2":
                characterEntry.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "???";
                break;

            default:
                characterEntry.transform.Find("Image").gameObject.GetComponent<Image>().color = Color.black;
                characterEntry.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = "???";
                characterEntry.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "???";
                break;
        }
    }

    // UI methods

    public void OpenCodex()
    {
        if (!codexOpen)
        {
            codexDisplay.SetActive(true);
            codexOpen = true;
        }  
    }

    public void CloseCodex()
    {
        codexDisplay.SetActive(false);

        if (codexOpen) 
        {
            foreach (string newUnlock in SaveManager.instance.newestCharacterUnlocks) 
            {
                characterEntries[newUnlock].transform.Find("Image").gameObject.GetComponent<Image>().color = Color.white;
            }

            SaveManager.instance.ClearNewestCharacterUnlocks();
            SaveManager.instance.Save();
        }

        codexOpen = false;
    }

    public void ToggleDenizensDisplay()
    {
        denizensDisplay.SetActive(true);
        bugsDisplay.SetActive(false);
    }

    public void ToggleBugsDisplay()
    {
        bugsDisplay.SetActive(true);
        denizensDisplay.SetActive(false);
    }
}
