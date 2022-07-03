using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RestrictMovementArthropodBehavior : ArthropodBehavior
{
    [SerializeField]
    List<Vector2Int> directions;

    [SerializeField]
    bool playerOnly = false;

    public override void Start(Arthropod arthropod)
    {
        base.Start(arthropod);

        arthropod.AddActionRule(
            new RestrictMovementActionRule(
                arthropod,
                arthropod.board,
                Arthropod.DefaultArthropodEnableCondition,
                directions,
                (BoardAction action) =>
                    (playerOnly ? action.boardObject is Player : true)
                    && action.boardObject.DistanceTo(arthropod) <= range
            )
        );
    }
}
