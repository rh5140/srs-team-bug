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

public class EFMActionRule : IActionRule
{
    public BoardObject creator { get; private set; }
    public Board board { get; private set; }

    public List<EnableCondition> enableConditions;
    private Filter filter;
    private Map map;

    public delegate bool EnableCondition(BoardObject creator, Board board);
    public delegate bool Filter(BoardAction action);
    public delegate BoardAction Map(BoardAction action);

    public EFMActionRule(
        BoardObject creator,
        Board board, 
        List<EnableCondition> enableConditions,
        Filter filter,
        Map map)
    {
        this.creator = creator;
        this.board = board;

        this.enableConditions = enableConditions;
        this.filter = filter;
        this.map = map;
    }

    public BoardAction Execute(BoardAction action)
    {
        if(enableConditions != null)
        {
            foreach (EnableCondition enableCondition in enableConditions)
            {
                if(enableCondition(creator, board) == false)
                {
                    return action;
                }
            }
        }

        // filter?.Invoke ?? true means invoke if filter is nonnull, otherwise true
        if (filter?.Invoke(action) ?? true)
        {
            return map(action);
        }
        return action;
    }
}