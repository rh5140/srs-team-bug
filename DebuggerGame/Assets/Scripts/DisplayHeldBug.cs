using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHeldBug : MonoBehaviour
{

    public Sprite transparent;

    Player player;
    Arthropod miscArthro;
    void Start()
    {
        player = GameObject.Find("PlayerController").GetComponent<Player>();
        miscArthro = null;
    }

    void Update()
    {
        miscArthro = player.heldArthropod;
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
