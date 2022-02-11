using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBug : Bug
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
                enableConditions: new List<EFMActionRule.EnableCondition> {
                    (BoardObject creator, Board board) => creator is Bug bug && !bug.isCaught && bug.rulesEnabled
                },
                filter: (BoardAction action) =>
                    action.boardObject is Player
                    && action is MovementAction movementAction
                    && movementAction.direction.normalized == Vector2.up,
                map: RuleMap
            )
        );
    }
}
