using System.Collections.Generic;

public class FlipControlsArthropod : Arthropod
{
    protected override void Start()
    {
        base.Start();

        board.actionRules.Add(
            new EFMActionRule(
                this,
                board,
                enableConditions: new List<EFMActionRule.EnableCondition>
                {
                    (boardObject, board) => boardObject is Arthropod arthro && !arthro.isCaught
                },
                filter: (action) => 
                    action.boardObject is Player
                    && action is MovementAction movement,
                map: (action) => new MovementAction(
                    action.boardObject,
                    -((MovementAction)action).direction
                )
            )
        );
    }
}
