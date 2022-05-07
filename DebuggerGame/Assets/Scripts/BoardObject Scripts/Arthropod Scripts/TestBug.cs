using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBug : Arthropod
{
    protected override void Start()
    {
        base.Start();

        AddActionRule(
            new EFMActionRule(
                this,
                board,
                enableCondition: (BoardObject creator, Board board) =>
                    creator is Arthropod arthropod
                    && !arthropod.isCaught
                    && arthropod.rulesEnabled,
                filter: (BoardAction action) =>
                    action.boardObject is Player
                    && action is MovementAction movementAction,
                map: action => new NullAction(action.boardObject)
            )
        );
    }
    protected override void OnEndTurn() {
        base.OnEndTurn();

        actions.Enqueue(new MovementAction(this, Vector2Int.right));
    }
}
