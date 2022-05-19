using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class DialogueBoxUI : MonoBehaviour
{
    public Sprite playerSprite;
    public Sprite otherSprite;

    [YarnCommand("SetSprite")]
    void SetSprite(string name)
    {
        if (name == "Player")
        {
            GetComponent<Image>().sprite = playerSprite;
        }
        else if (name == "Other")
        {
            GetComponent<Image>().sprite = otherSprite;
        }
        else
        {
            GetComponent<Image>().sprite = playerSprite;
        }
    }

}
