using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Codex : MonoBehaviour
{
    public static bool codexOpen = false;
    public GameObject codexDisplay;
    public GameObject denizensDisplay;
    public GameObject bugsDisplay;

    // Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    // void Start() {}
    // Update is called every frame, if the MonoBehaviour is enabled.
    // void Update() {}

    // Functionality methods
    // (Andrew write your code here, can delete this line after you've read this)

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
