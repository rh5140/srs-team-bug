using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BoardObject
{
    public float moveSpeed;
    private Vector2 targetPos;
    private Vector2 input;

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        //Checks for input. If horizontal input is detected, ignore vertical
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = (input.x == 0) ? Input.GetAxisRaw("Vertical") : 0;
        
        //Calulate targetPos based on moveSpeed and time alloted to actions
        targetPos = input * (moveSpeed * Board.TimePerAction);

        //Restrict adding MovementActions to when the Player is near the old targetPos
        if (((Vector3)targetPos - transform.position).sqrMagnitude <= Mathf.Epsilon)
        {
            base.actions.Enqueue(new MovementAction(this, targetPos, moveSpeed));
        }

        base.Update();
    }
}
