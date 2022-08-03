using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorPauser : MonoBehaviour
{
    private Animator animator;
    private float defaultSpeed;

    private void Start()
    {
        animator = GetComponent<Animator>();
        defaultSpeed = animator.speed;
        Debug.Log(Board.instance);
        Board.instance.GameStateChangeEvent.AddListener(OnGameStateChange);
    }

    private void OnDestroy()
    {
        if(Board.instance)
            Board.instance.GameStateChangeEvent.RemoveListener(OnGameStateChange);
    }

    private void OnGameStateChange()
    {
        if(Board.instance.gameState == Board.GameState.Paused)
        {
            animator.speed = 0;
        }
        else
        {
            animator.speed = defaultSpeed;
        }
    }
}
