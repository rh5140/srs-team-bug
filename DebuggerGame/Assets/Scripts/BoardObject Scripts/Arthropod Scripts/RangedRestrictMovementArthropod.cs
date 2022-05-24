using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RangedRestrictMovementArthropod : RangedBug
{
    [SerializeField]
    List<Vector2Int> directions;

    [SerializeField]
    bool playerOnly = false;

    override protected void Start()
    {
        base.Start();

        //ArrayList directions = new ArrayList();
        AddActionRule(
            new RestrictMovementActionRule(
                this,
                board,
                DefaultArthropodEnableCondition,
                directions,
                (BoardAction action) =>
                    playerOnly ? action.boardObject is Player : true
            )
        );
    }
}

