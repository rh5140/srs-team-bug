using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject Controls;
    public GameObject Credits;

    public void ShowControls()
    {
        Controls.SetActive(true);
    }

    public void CloseControls()
    {
        Controls.SetActive(false);
    }

    public void ShowCredits()
    {
        Credits.SetActive(true);
    }

    public void CloseCredits()
    {
        Credits.SetActive(false);
    }
}
