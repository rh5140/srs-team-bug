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

    public List<List<EnablePredicate>> enableConditions;
    private Filter filter;
    private Map map;

    public delegate bool EnablePredicate(BoardObject creator, Board board);
    public delegate bool Filter(BoardAction action);
    public delegate BoardAction Map(BoardAction action);

    public EFMActionRule(
        BoardObject creator,
        Board board, 
        List<List<EnablePredicate>> enableConditions,
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
        bool enabled = true;
        if(enableConditions != null)
        {
            foreach (List<EnablePredicate> predicateSet in enableConditions)
            {
                bool predicateSetResult = false;
                foreach (EnablePredicate predicate in predicateSet)
                {
                    predicateSetResult |= predicate(creator, board);
                }
                enabled &= predicateSetResult;
            }
        }

        if(enabled)
        {
            if (filter?.Invoke(action) ?? true)
            {
                return map(action);
            }
        }
        return action;
    }
}