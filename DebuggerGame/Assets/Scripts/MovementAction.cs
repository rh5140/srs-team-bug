using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAction : BoardAction
{
    public float moveSpeed;
    private Vector2 targetPos;
    private bool isMoving;

    public MovementAction(BoardObject boardObject, Vector2 targetPos, float moveSpeed) : base(boardObject)
    {
        this.targetPos = targetPos;
        this.moveSpeed = moveSpeed;
    }

    //Set coordinate of boardObject to be the new location
    public override void ExecuteStart()
    {
        base.ExecuteStart();

        base.boardObject.coordinate += new Vector2Int((int)targetPos.x, (int)targetPos.y);

        isMoving = true;
    }

    //Move boardObject towards targetPos
    public override void ExecuteUpdate(float progress)
    {
        base.ExecuteUpdate(progress);

        base.boardObject.transform.position = Vector3.MoveTowards(base.boardObject.transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    public override void ExecuteFinish()
    {
        base.ExecuteFinish();

        isMoving = false;
    }
}
