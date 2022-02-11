using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BoardObject
{
    protected override void Update()
    {
        base.Update();
        if (board.lastBoardEvent == Board.EventState.StartTurn)
        {
            // only move if during a turn

            Vector2 input = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );

            // check if input is nonzero
            // if it is, move in whatever direction in with component velocity 1
            Vector2Int direction = new Vector2Int(
                Mathf.Approximately(input.x, 0f) ? 0 : 1 * (int)Mathf.Sign(input.x),
                Mathf.Approximately(input.y, 0f) ? 0 : 1 * (int)Mathf.Sign(input.y)
            );

            // if x direction is nonzero, then set y direction to zero (no diagonal movement)
            direction.y = direction.x == 0 ? direction.y : 0;

            if (!Mathf.Approximately(direction.magnitude, 0f))
            {
                // if we have input, add an action and then end turn
                actions.Enqueue(new MovementAction(this, direction));
                board.EndTurn();
            }
        }
    }
}
