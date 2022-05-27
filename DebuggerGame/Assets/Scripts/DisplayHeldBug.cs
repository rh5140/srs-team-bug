using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHeldBug : MonoBehaviour
{
    public Sprite transparent;
    void OnEnable()
    {
        Arthropod miscArthro = GameObject.Find("PlayerController").GetComponent<Player>().heldArthropod;
        Sprite bugSprite;
        if (miscArthro == null)
        {
            transform.GetComponent<Image>().sprite = transparent;
        }
        else
        {
            bugSprite = miscArthro.GetComponentInChildren<SpriteRenderer>().sprite;
            transform.GetComponent<Image>().sprite = bugSprite;
        }

    }
}
