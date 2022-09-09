using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Represents a movement from a coordinate to an adjacent coordinate.
/// Currently takes in a direction and lerps to the initial position + the direction.
/// Sets BoardObject.coordinate *after* it finishes executing.
/// </summary>
public class MovementAction : BoardAction
{
    public readonly Vector2Int direction;

    /// <summary>
    /// Initial position when execution hasn't started yet.
    /// Used if action aborted.
    /// </summary>
    private Vector2 initialPosition;

    public MovementAction(BoardObject boardObject, Vector2Int direction) : base(boardObject)
    {
        // TODO: Ensure that direction is a unit vector along an axis
        this.direction = direction;
    }

    public override void ExecuteStart()
    {
        base.ExecuteStart();
        initialPosition = boardObject.gameObject.transform.position;
    }

    public override void ExecuteUpdate(float progress)
    {
        base.ExecuteUpdate(progress);
        
        // linearly go to final position
        // since ExecuteUpdate is GUARENTEED to be called with progress
        // being 1.0f at the end, this will end with the final position
        boardObject.gameObject.transform.position = Vector2.Lerp(
            initialPosition,
            initialPosition + direction,
            progress
        );
    }


    public override void ExecuteFinish()
    {
        base.ExecuteFinish();
        
        Vector2Int oldCoord = boardObject.coordinate;

        boardObject.coordinate = new Vector2Int((int)initialPosition.x, (int)initialPosition.y) + direction;
        
        Arthropod ruleCreator;
        if (boardObject is Arthropod) {
            ruleCreator = (Arthropod)boardObject;
        }

        if (boardObject is Arthropod && (boardObject as Arthropod).restrictMovementArthropodBehavior.range > -1) {
            Arthropod creator = (Arthropod)boardObject;
            int creatorRange = creator.restrictMovementArthropodBehavior.range;

            for(int i = oldCoord.x - creatorRange; i < oldCoord.x + creatorRange + 1; i++) {
                for(int j = oldCoord.y - creatorRange; j < oldCoord.y + creatorRange + 1; j++) {
                    if (i < Board.instance.width && i >= 0 && j < Board.instance.height && j >= 0) {
                        Board.instance.outlineMap.DeactivateTile(i, j);
                    }
                }
            }   

            for(int i = boardObject.coordinate.x - creatorRange; i < boardObject.coordinate.x + creatorRange + 1; i++) {
                for(int j = boardObject.coordinate.y - creatorRange; j < boardObject.coordinate.y + creatorRange + 1; j++) {
                    if (i < Board.instance.width && i >= 0 && j < Board.instance.height && j >= 0) {
                        Board.instance.outlineMap.ActivateTile(i, j);
                    }
                }
            }
        }
    }


    public override void Abort()
    {
        base.Abort();
        boardObject.gameObject.transform.position = initialPosition;
    }
}
