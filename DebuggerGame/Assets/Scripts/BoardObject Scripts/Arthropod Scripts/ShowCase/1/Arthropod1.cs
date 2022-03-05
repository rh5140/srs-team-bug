using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level1
{
    public class Arthropod1 : Arthropod
    {
        public override void Catch()
        {
            base.Catch();
        }

        protected override void Start()
        {
            base.Start();

            board.actionRules.Add(
                new EFMActionRule(
                    this,
                    board,
                    enableConditions: new List<EFMActionRule.EnableCondition>{
                        (boardObject, board) =>
                            boardObject is Arthropod arthropod
                            && !arthropod.isCaught
                    },
                    filter: (boardAction) => 
                        boardAction.boardObject is Player
                        && boardAction is MovementAction movementAction
                        && movementAction.direction.x > 0,
                    map: (boardAction) => new NullAction(boardAction.boardObject)
                )
            );
        }
    }

}