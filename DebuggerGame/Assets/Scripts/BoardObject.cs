using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BoardObject : MonoBehaviour
{
    public Board board;


    protected List<Action> actions = new List<Action>();
    // ASSUME HAS LENGTH OF ONE, WITH ONE MovementAction


    virtual protected void Start()
    {
        board = GetComponentInParent<Board>();
    }


    virtual protected void EndTurn()
    {
        // TODO: Interpret actions, move using Board.timeSinceEndTurn
        throw new System.NotImplementedException();
    }
}
