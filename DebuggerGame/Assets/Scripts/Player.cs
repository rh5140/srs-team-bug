using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BoardObject
{
    public InventorySystem collection;

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

    protected override void OnStartTurn()
    {
        base.OnStartTurn();

        /*
        Note: The bug overlap has to be checked for at the beginning of the turn since position has to update before we check if player is overlapping,
        however there is currently no implementation for actions to be executed at the beginning of turn 
        (they only currently execute at the execute phase), so this behavior is hardcoded for now in a slapdash manner.
        i.e bugs may play animations or have certain actions before being cause, which is not accounted for 
        in this implementation. Deletion of the bug during OnStartTurn might also mess with the logic of other
        board objects that rely on the existence of the bug(that is getting caught) to function.

        TODO: Action based implementation of the overlap checking.
        */
        //actions.Enqueue(DetectBugOverlap()); ?

        Collider2D[] objectsOverlap = null;
        objectsOverlap = Physics2D.OverlapBoxAll((Vector2)this.transform.position, new Vector2(0.1f, 0.1f), 0f, int.MinValue, int.MaxValue);
        foreach (Collider2D newCol in objectsOverlap)
        {
            if (newCol.gameObject.tag.Equals("Bug")) //temporary identification for bug gameobjects (REPLACE THIS) 
            {
                //newCol.gameObject.GetComponent<>().OnCaught(); Call the OnCaught Method for bugs, since the way to access the class of the bug is unknown, left blank.
                board.BugCountUpdate();
                
                break;
            }
        }



        

    }

    /*
    /// <summary>
    /// DetectBugOverlap detects if player is overlapping a bug, then returns an action that will collect the bug.
    /// </summary>
    /// <returns></returns>
    /// 
    private BoardAction DetectBugOverlap()
    {
        //TODO: Action based implementation
        return null;
    }
    */


    private void OnApplicationQuit()
    {
        collection?.Container.Clear();
    }
}
