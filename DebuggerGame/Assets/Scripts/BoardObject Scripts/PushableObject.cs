using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : BoardObject
{
    public bool Push(Vector2Int direction) {
        BoardObject objectAtNewCoordinate = Board.instance.GetBoardObjectAtCoordinate(this.coordinate + direction);

        if(objectAtNewCoordinate is PushableObject) {
            PushableObject pushableAtNewCoordinate = (PushableObject)objectAtNewCoordinate;
            if(pushableAtNewCoordinate.Push(direction)) {
                actions.Enqueue(new MovementAction(this, direction));
                return true;
            }
            return false;
        }
        else if(objectAtNewCoordinate == null) {
            actions.Enqueue(new MovementAction(this, direction));
            return true;
        }
        return false;
    }
}
