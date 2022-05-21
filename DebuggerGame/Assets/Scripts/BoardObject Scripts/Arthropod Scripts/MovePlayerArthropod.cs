using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerArthropod : Arthropod
{
    [SerializeField]
    protected List<Vector2Int> movements = new List<Vector2Int>();

    override protected void OnEndTurn()
    {
        base.OnEndTurn();

        if (rulesEnabled)
        {
            var player = board.GetBoardObjectOfType<Player>();
            foreach (var direction in movements)
            {
                player.actions.Enqueue(new MovementAction(
                    player, direction
                ));
            }
        }
    }
}
