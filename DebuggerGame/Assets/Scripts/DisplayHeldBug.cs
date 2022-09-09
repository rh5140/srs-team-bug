using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHeldBug : MonoBehaviour
{

    public Sprite transparent;

    public Animator animator;

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
        animator.SetBool("empty", true);
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
            animator.SetBool("empty", true);
            transform.GetComponent<Image>().sprite = transparent;
        }
        else
        {
            animator.SetBool("empty", false);
            bugSprite = miscArthro.GetComponentInChildren<SpriteRenderer>().sprite;
            transform.GetComponent<Image>().sprite = bugSprite;
        }

    }

}
