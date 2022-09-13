using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingTutorialAnimations : MonoBehaviour 
{
    public Animator animator;
    public Vector2 movement;

    void Start()
    {
        animator.SetBool("right", true);
        animator.SetBool("left", false);
        animator.SetBool("up", false);
        animator.SetBool("down", false);
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if (movement.x < 0) // left
        {
            animator.SetBool("right", false);
            animator.SetBool("left", true);
            animator.SetBool("up", false);
            animator.SetBool("down", false);
        }
        if (movement.x > 0) // right
        {
            animator.SetBool("right", true);
            animator.SetBool("left", false);
            animator.SetBool("up", false);
            animator.SetBool("down", false);
        }
        if (movement.y > 0) // up
        {
            animator.SetBool("right", false);
            animator.SetBool("left", false);
            animator.SetBool("up", true);
            animator.SetBool("down", false);
        }
        if (movement.y < 0) // down
        {
            animator.SetBool("right", false);
            animator.SetBool("left", false);
            animator.SetBool("up", false);
            animator.SetBool("down", true);
        }
    }
}