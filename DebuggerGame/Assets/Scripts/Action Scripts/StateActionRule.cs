using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateActionRule
{
    public BoardObject creator { get; }
    public Board board { get; }

    public BoardAction Execute(BoardAction action, int? offset);
}

public abstract class EFStateActionRule : IStateActionRule
{
    public delegate bool EnableCondition(BoardObject creator, Board board, int? offset);
    public delegate bool Filter(BoardAction action, int? offset);

    public BoardObject creator { get; protected set; }
    public Board board { get; protected set; }

    protected EnableCondition enableCondition;
    protected Filter filter;

    public EFStateActionRule(
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

    public abstract BoardAction Execute(BoardAction action, int? offset);
}

public class EFMStateActionRule : EFStateActionRule
{
    public delegate BoardAction Map(BoardAction action, int? offset);

    private Map map;

    public EFMStateActionRule(
        BoardObject creator,
        Board board,
        EnableCondition enableCondition,
        Filter filter,
        Map map) : base(creator, board, enableCondition, filter)
    {
        this.map = map;
    }

    public override BoardAction Execute(BoardAction action, int? offset)
    {
        if (!enableCondition?.Invoke(creator, board, offset) ?? false)
        {
            return action;
        }

        // filter?.Invoke ?? true means invoke if filter is nonnull, otherwise true
        if (filter?.Invoke(action, offset) ?? true)
        {
            return map(action, offset);
        }
        return action;
    }
}


public class EFStateActionDeleterRule : EFMStateActionRule
{
    public EFStateActionDeleterRule(
        BoardObject creator,
        Board board,
        EnableCondition enableCondition,
        Filter filter) : base(
            creator,
            board,
            enableCondition,
            filter,
            (BoardAction action, int? offset) => new NullAction(action.boardObject)
        )
    { }
}