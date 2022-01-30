using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#nullable enable
public class RuleBuilder
{
    private class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T o1, T o2)
        {
            return Object.ReferenceEquals(o2, o2);
        }

        public int GetHashCode(T o)
        {
            return o?.GetHashCode() ?? 0;
        }
    }

    private BoardObject creator;
    private List<Rule.BoardActionFilter> filters = new List<Rule.BoardActionFilter>();
    private List<Rule.EnableCondition> enableConditions = new List<Rule.EnableCondition>();
    private Queue<Rule.BoardActionMap> maps = new Queue<Rule.BoardActionMap>();


    public RuleBuilder(BoardObject creator)
    {
        this.creator = creator;
    }


    public RuleBuilder For<T>(T targetBoardObject)
        where T : BoardObject
    {
        filters.Add((BoardObject boardObject, BoardAction? boardAction) => Object.ReferenceEquals(boardObject, targetBoardObject));
        return this;
    }


    public RuleBuilder ForAll<T>()
    {
        filters.Add((BoardObject boardObject, BoardAction? boardAction) => boardObject is T);
        return this;
    }


    public RuleBuilder ForSome(IEnumerable<BoardObject> targetBoardObjects)
    {
        filters.Add(
            (BoardObject boardObject, BoardAction? boardAction) => targetBoardObjects.Contains(
                boardObject,
                new ReferenceEqualityComparer<BoardObject>()
            )
        );
        return this;
    }


    public RuleBuilder While(Rule.EnableCondition condition)
    {
        enableConditions.Add(condition);
        return this;
    }


    public RuleBuilder WhileNotCaptured()
    {
        if (!(creator is Bug))
        {
            Debug.LogError("'WhileNotCaptured()' called from a nonbug. Please add the target bug as an argument, otherwise, this condition will be disabled.");
            return this;

        }
        enableConditions.Add(
            (BoardObject creator) =>  creator is Bug bug ? !bug.isCaught : false
        );
        return this;
    }


    public RuleBuilder WhileNotCaptured(Bug bug)
    {
        enableConditions.Add(
            (BoardObject creator) => !bug.isCaught
        );
        return this;
    }


    public RuleBuilder DisableMovementInDirection(Vector2 direction)
    {
        filters.Add(
            (BoardObject boardObject, BoardAction? boardAction) => {
                return boardAction is MovementAction movementAction
                ? !(movementAction.direction.normalized == direction.normalized)
                : true;
            }
        );
        return this;
    }


    public RuleBuilder ReverseMovementInDirection(Vector2 direction)
    {
        maps.Enqueue(
            (BoardObject boardObject, BoardAction? boardAction) => {
                return boardAction is MovementAction movementAction
                ? (
                    movementAction.direction.normalized == direction.normalized
                    ? new MovementAction(movementAction.boardObject, -movementAction.direction)
                    : movementAction
                ) : boardAction;
            }
        );
        return this;
    }


    public Rule Build()
    {
        return new Rule(creator, enableConditions, filters, maps);
    }
}

public class Rule
{
    public BoardObject creator { get; private set; }

    public bool? enableOverride = null;

    /// <summary>
    /// Filters that take in an action and return true if it is to follow through
    /// or false if it is to be deleted.
    /// </summary>
    private List<BoardActionFilter> filters;
    private List<EnableCondition> enableConditions;
    private Queue<BoardActionMap> maps;


    public delegate bool BoardActionFilter(BoardObject boardObject, BoardAction? boardAction);
    public delegate bool EnableCondition(BoardObject creator);
    public delegate BoardAction? BoardActionMap(BoardObject boardObject, BoardAction? boardAction);


    public static BoardAction? IdentityMap(BoardObject boardObject, BoardAction? boardAction)
    {
        return boardAction;
    }


    public static RuleBuilder Builder(BoardObject creator)
    {
        return new RuleBuilder(creator);
    }


    public Rule(
        BoardObject creator,
        List<EnableCondition> enableConditions,
        List<BoardActionFilter> filters,
        Queue<BoardActionMap> maps
    )
    {
        this.enableConditions = enableConditions;
        this.creator = creator;
        this.filters = filters;
        this.maps = maps;
    }


    public BoardAction? Apply(BoardObject boardObject, BoardAction? boardAction)
    {
        if (this.enableOverride is bool enableOverride)
        {
            if (enableOverride == false)
            {
                return boardAction;
            }
        }
        else
        {
            foreach (var enableCondition in enableConditions)
            {
                if (!enableCondition(creator))
                {
                    return boardAction;
                }
            }
        }
        foreach (var filter in filters)
        {
            // If any of the filters say to disclude the rule, return null
            if (!filter(boardObject, boardAction))
            {
                return null;
            }   
        }
        BoardAction? currentAction = boardAction;
        foreach (var map in maps)
        {
            // map the action by each mapper in order
            currentAction = map(boardObject, currentAction);
        }
        return currentAction;
    }
}
#nullable restore
