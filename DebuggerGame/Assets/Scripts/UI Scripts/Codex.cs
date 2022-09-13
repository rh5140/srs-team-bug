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
    public Component[] pages;
    private int currentPage = 0; // MIGHT WANT TO MOVE THIS ?

    public GameObject greyedLeftButton;
    public GameObject greyedRightButton;
    public GameObject exclamationMark;

    // entries
    public GameObject basicFlyEntry;
    public GameObject elderFlytrapEntry;
    public GameObject cryingCloverEntry;
    public GameObject weirdFlyEntry;
    public GameObject flyKingEntry;
    public GameObject theAcademyEntry;
    public GameObject theBreachEntry;
    public GameObject whoAmIEntry;

    // Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    void Start() 
    {
        pages = codexDisplay.GetComponentsInChildren<Canvas>(true);
        Debug.Log(pages.Length);

        string[] characterNames = {"elderFlytrap", "cryingClover1", "cryingClover2", "flyKing", "basicFly", "weirdFly", "theAcademy", "theBreach", "whoAmI"};

        characterEntries.Add("elderFlytrap", elderFlytrapEntry);
        characterEntries.Add("cryingClover1", cryingCloverEntry);
        characterEntries.Add("cryingClover2", cryingCloverEntry);
        characterEntries.Add("flyKing", flyKingEntry);
        characterEntries.Add("basicFly", basicFlyEntry);
        characterEntries.Add("weirdFly", weirdFlyEntry);
        characterEntries.Add("theAcademy", theAcademyEntry);
        characterEntries.Add("theBreach", theBreachEntry);
        characterEntries.Add("whoAmI", whoAmIEntry);

        foreach (string characterName in characterNames)
        {
            if (!SaveManager.instance.unlockedCharacters.Contains(characterName))
            {
                HideEntry(characterEntries[characterName], characterName);
            }
        }

        foreach (string newUnlock in SaveManager.instance.newestCharacterUnlocks) 
        {
            exclamationMark.SetActive(true);
            // characterEntries[newUnlock].transform.Find("Image").gameObject.GetComponent<Image>().color = Color.red;
        }
    }

    // Functionality methods

    public void HideEntry(GameObject characterEntry, string name)
    {
        switch (name) 
        {
            case "basicFly":
                characterEntry.transform.Find("Image").gameObject.GetComponent<Image>().color = Color.black;
                characterEntry.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = "???";
                characterEntry.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "I still have a lot to learn about the world...";
                characterEntry.transform.Find("Extra1").gameObject.GetComponent<TextMeshProUGUI>().text = "";
                characterEntry.transform.Find("Extra2").gameObject.GetComponent<TextMeshProUGUI>().text = "";
                break;

            case "flyKing":
                characterEntry.transform.Find("Image").gameObject.GetComponent<Image>().color = Color.black;
                characterEntry.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = "???";
                characterEntry.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Bzzzzz... something's missing...";
                characterEntry.transform.Find("Extra1").gameObject.GetComponent<TextMeshProUGUI>().text = "";
                characterEntry.transform.Find("Extra2").gameObject.GetComponent<TextMeshProUGUI>().text = "";
                break;

            case "elderFlytrap":
                characterEntry.transform.Find("Image").gameObject.GetComponent<Image>().color = Color.black;
                characterEntry.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = "???";
                characterEntry.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "I still have a lot to learn about the world...";
                characterEntry.transform.Find("Extra1").gameObject.GetComponent<TextMeshProUGUI>().text = "";
                break;

            case "cryingClover1":
                characterEntry.transform.Find("Image").gameObject.GetComponent<Image>().color = Color.black;
                characterEntry.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = "???";
                characterEntry.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "I still have a lot to learn about the world...";
                characterEntry.transform.Find("Extra1").gameObject.GetComponent<TextMeshProUGUI>().text = "";
                break;

            case "cryingClover2":
                characterEntry.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "I'm helping this child now...";
                characterEntry.transform.Find("Extra1").gameObject.GetComponent<TextMeshProUGUI>().text = "";
                break;

            default:
                characterEntry.transform.Find("Image").gameObject.GetComponent<Image>().color = Color.black;
                characterEntry.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = "???";
                characterEntry.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "I still have a lot to learn about the world...";
                break;
        }
    }

    // UI methods

    public void CodexButton()
    {
        // Open Codex
        if (!codexOpen)
        {
            codexDisplay.SetActive(true);
            codexOpen = true;
        }  

        // Close Codex
        else
        {
            foreach (string newUnlock in SaveManager.instance.newestCharacterUnlocks) 
            {
                characterEntries[newUnlock].transform.Find("Image").gameObject.GetComponent<Image>().color = Color.white;
            }
            SaveManager.instance.ClearNewestCharacterUnlocks();
            SaveManager.instance.Save();

            codexDisplay.SetActive(false);
            codexOpen = false;

            exclamationMark.SetActive(false);
        }
    }

    public void FlipRight()
    {
        pages[currentPage].GetComponent<Canvas>().enabled = false;
        currentPage = Mathf.Clamp(currentPage + 1, 0, pages.Length - 1);
        pages[currentPage].GetComponent<Canvas>().enabled = true;
        if (currentPage == pages.Length - 1)
        {
            greyedRightButton.SetActive(true);
            greyedLeftButton.SetActive(false);
        }
        else
        {
            greyedRightButton.SetActive(false);
            greyedLeftButton.SetActive(false);
        }
    }

    public void FlipLeft()
    {
        pages[currentPage].GetComponent<Canvas>().enabled = false;
        currentPage = Mathf.Clamp(currentPage - 1, 0, pages.Length - 1);
        pages[currentPage].GetComponent<Canvas>().enabled = true;
        if (currentPage == 0)
        {
            greyedLeftButton.SetActive(true);
            greyedRightButton.SetActive(false);
        }
        else
        {
            greyedLeftButton.SetActive(false);
            greyedRightButton.SetActive(false);
        }
    }
}
