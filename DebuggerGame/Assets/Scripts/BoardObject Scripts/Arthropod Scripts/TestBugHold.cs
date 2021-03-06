using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBugHold : Arthropod
{
    BoardAction RuleMap(BoardAction action)
    {
        return new NullAction(action.boardObject);
    }

    protected override void Start()
    {
        base.Start();

        AddActionRule(
            new EFMActionRule(
                this,
                board,
                enableCondition: (BoardObject creator, Board board)
                        => creator is Arthropod arthropod && arthropod.rulesEnabled,
                filter: (BoardAction action) =>
                    action.boardObject is Player
                    && action is MovementAction movementAction
                    && movementAction.direction.y > 0,
                map: RuleMap
            )
        );
    }

    protected override void Update()
    {
        base.Update();
        Debug.Log("Active Updating");
    }


}
