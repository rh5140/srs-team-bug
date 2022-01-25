using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public readonly ActionType type;

    public Action(ActionType type)
    {
        this.type = type;
    }

    public enum ActionType
    {
        Movement,
    };
}

public class MovementAction : Action
{
    public readonly Vector2Int direction;

    public MovementAction(Vector2Int direction) : base(ActionType.Movement)
    {
        // TODO: Ensure that direction is a unit vector along an axis
        this.direction = direction;
    }
}