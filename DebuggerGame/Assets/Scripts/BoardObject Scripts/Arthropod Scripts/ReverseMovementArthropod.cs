using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseMovementArthropod : Arthropod
{
    [SerializeField]
    List<Vector2Int> directions;
    
    [SerializeField]
    bool playerOnly = true;

    override protected void Start()
    {
        base.Start();

        //ArrayList directions = new ArrayList();
        AddActionRule(
            new ReverseMovementActionRule(
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