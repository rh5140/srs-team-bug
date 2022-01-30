using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAction : BoardAction
{
    public Vector2 direction { get; private set; }
    private Vector2 initialPosition;

    public MovementAction(BoardObject boardObject, Vector2 direction) : base(boardObject)
    {
        this.direction = direction;
    }

    //Set coordinate of boardObject to be the new location
    public override void ExecuteStart()
    {
        base.ExecuteStart();
        initialPosition = boardObject.transform.position;
    }

    //Move boardObject towards targetPos
    public override void ExecuteUpdate(float progress)
    {
        base.ExecuteUpdate(progress);

        boardObject.transform.position = Vector2.Lerp(
            initialPosition,
            initialPosition + direction,
            progress
        );
    }

    public override void ExecuteFinish()
    {
        base.ExecuteFinish();
        boardObject.transform.position = initialPosition + direction;
    }
}
