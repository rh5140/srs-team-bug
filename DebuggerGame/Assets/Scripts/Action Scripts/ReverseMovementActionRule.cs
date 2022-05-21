using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseMovementActionRule : EFMActionRule
{
    public ReverseMovementActionRule(
        BoardObject creator,
        Board board,
        EnableCondition enableCondition,
        IEnumerable<Vector2Int> directions,
        Filter filter = null) : base(
            creator,
            board,
            enableCondition,
            (BoardAction action) =>
            {
                if (action is MovementAction movementAction)
                {
                    foreach (Vector2Int direction in directions)
                    {
                        var directionNorm = Vector2Int.RoundToInt((Vector2)direction / direction.magnitude);
                        var movementActionDirectionNorm = Vector2Int.RoundToInt((Vector2)movementAction.direction / movementAction.direction.magnitude);
                        if (directionNorm == movementActionDirectionNorm || directionNorm == -movementActionDirectionNorm)
                        {
                            return filter?.Invoke(action) ?? true;
                        }
                    }
                }
                return false;
            },
            (BoardAction action) => new MovementAction(action.boardObject, -(action as MovementAction).direction)
        )
    { }
}
