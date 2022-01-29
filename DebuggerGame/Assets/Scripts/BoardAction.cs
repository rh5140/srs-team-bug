using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BoardAction
{
    protected BoardObject boardObject;
    public BoardAction(BoardObject boardObject)
    {
        this.boardObject = boardObject;
    }

    public virtual void Abort()
    { }

    public virtual void ExecuteStart()
    { }

    public virtual void ExecuteUpdate(float progress)
    { }

    public virtual void ExecuteFinish()
    { }
}