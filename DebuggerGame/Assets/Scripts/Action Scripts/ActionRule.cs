using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionRule
{
    public BoardObject creator { get; }
    public Board board { get; }

    public BoardAction Execute(BoardAction action);
}

public abstract class EFActionRule : IActionRule
{
    public delegate bool EnableCondition(BoardObject creator, Board board);
    public delegate bool Filter(BoardAction action);

    public BoardObject creator { get; protected set; }
    public Board board { get; protected set; }

    protected EnableCondition enableCondition;
    protected Filter filter;

    public EFActionRule(
        BoardObject creator,
        Board board,
        EnableCondition enableCondition,
        Filter filter)
    {
        this.creator = creator;
        this.board = board;

        this.enableCondition = enableCondition;
        this.filter = filter;
    }

    public abstract BoardAction Execute(BoardAction action);
}

public class EFMActionRule : EFActionRule
{
    public delegate BoardAction Map(BoardAction action);

    private Map map;

    public EFMActionRule(
        BoardObject creator,
        Board board, 
        EnableCondition enableCondition,
        Filter filter,
        Map map) : base(creator, board, enableCondition, filter)
    {
        this.map = map;
    }

    public override BoardAction Execute(BoardAction action)
    {
        if(!enableCondition?.Invoke(creator, board) ?? false)
        {
            return action;
        }

        // filter?.Invoke ?? true means invoke if filter is nonnull, otherwise true
        if (filter?.Invoke(action) ?? true)
        {
            return map(action);
        }
        return action;
    }
}


public class EFActionDeleterRule : EFMActionRule
{
    public EFActionDeleterRule(
        BoardObject creator,
        Board board,
        EnableCondition enableCondition,
        Filter filter) : base(
            creator,
            board,
            enableCondition,
            filter,
            (BoardAction action) => new NullAction(action.boardObject)
        )
    { }
}