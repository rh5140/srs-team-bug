using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Codex : MonoBehaviour
{
    //public static Codex instance = null;

    public string[] characterNames = {"elderFlytrap","cryingClover","flyKing","basicFly","weirdFly"};

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

        foreach (string characterName in characterNames)
        {
            if (SaveManager.instance.unlockedCharacters.Contains(characterName))
            {
                AddToCodex(characterName);
            }
        }
    }

    // Update is called every frame, if the MonoBehaviour is enabled.
    /*void Update() 
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddToCodex("elderFlytrap");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            AddToCodex("cryingClover");
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            AddToCodex("flyKing");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            AddToCodex("basicFly");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            AddToCodex("weirdFly");
        }
    }*/

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Save Menu"){
            // Destroy the gameobject this script is attached to
            Destroy(this.gameObject);
        }
    }

    /*private void Awake()
    {
        //Singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }*/

    // Functionality methods

    public void AddToCodex(string characterName)
    {
        UnhideCharacter(characterName);
    }

    public void UnhideCharacter(string characterName)
    {

        switch (characterName)
        {
            case "elderFlytrap":
                elderFlytrapEntry.transform.Find("Image").gameObject.GetComponent<Image>().color = Color.white;
                elderFlytrapEntry.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = "Elder Flytrap";
                elderFlytrapEntry.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "[INSERT DESCRIPTION]";
                break;
            
            case "cryingClover":
                cryingCloverEntry.transform.Find("Image").gameObject.GetComponent<Image>().color = Color.white;
                cryingCloverEntry.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = "Crying Clover";
                cryingCloverEntry.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "[INSERT DESCRIPTION]";
                break;
            
            case "flyKing":
                flyKingEntry.transform.Find("Image").gameObject.GetComponent<Image>().color = Color.white;
                flyKingEntry.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = "Fly King";
                flyKingEntry.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "[INSERT DESCRIPTION]";
                break;

            case "basicFly":
                basicFlyEntry.transform.Find("Image").gameObject.GetComponent<Image>().color = Color.white;
                basicFlyEntry.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = "Basic Fly";
                basicFlyEntry.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "[INSERT DESCRIPTION]";
                break;
            
            case "weirdFly":
                weirdFlyEntry.transform.Find("Image").gameObject.GetComponent<Image>().color = Color.white;
                weirdFlyEntry.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = "Weird Fly";
                weirdFlyEntry.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "[INSERT DESCRIPTION]";
                break;

            default:
                string errorString = "No such character with name " + characterName;
                Debug.Log(errorString);
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
