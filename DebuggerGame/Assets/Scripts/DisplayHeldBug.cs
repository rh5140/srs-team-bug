using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHeldBug : MonoBehaviour
{

    public Sprite transparent;

    GameObject playerObj;
    Player player;
    Arthropod miscArthro;
    Board board;
    void Start()
    {
        playerObj = GameObject.Find("PlayerController");
        if (playerObj == null)
        {
            return;
        }
        player = playerObj.GetComponent<Player>();
        miscArthro = null;
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

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
