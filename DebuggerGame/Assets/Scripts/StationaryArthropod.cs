using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryArthropod : Arthropod
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
                    (BoardObject creator, Board board)
                    => creator is Arthropod arthropod && !arthropod.isCaught && arthropod.rulesEnabled
                },

                filter: (BoardAction action) =>
                    action.boardObject is Player
                    && action is MovementAction movementAction,
                map: RuleMap
            )
        );
    }
}
