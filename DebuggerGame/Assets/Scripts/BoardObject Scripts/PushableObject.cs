using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : BoardObject
{
    public bool Push(Vector2Int direction, int? actionOffset) {
        Vector2Int newCoordinate = this.coordinate + direction;
        BoardObject objectAtNewCoordinate = Board.instance.GetBoardObjectAtCoordinate(this.coordinate + direction);

        
        if(newCoordinate.x < 0 || newCoordinate.x >= Board.instance.width
        || newCoordinate.y < 0 || newCoordinate.y >= Board.instance.height) {
            return false;
        }
        else if(objectAtNewCoordinate is PushableObject) {
            PushableObject pushableAtNewCoordinate = (PushableObject)objectAtNewCoordinate;
            if(pushableAtNewCoordinate.Push(direction, actionOffset)) {
                AddActionMidExecution(new MovementAction(this, direction), actionOffset);
                return true;
            }
            return false;
        }
        else if(objectAtNewCoordinate == null) {
            AddActionMidExecution(new MovementAction(this, direction), actionOffset);
            return true;
        }
        return false;
    }
}
