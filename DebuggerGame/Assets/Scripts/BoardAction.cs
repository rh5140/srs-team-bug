using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoardAction
{
    public readonly BoardObject boardObject;

    /// <summary>
    /// True if the action takes time. 
    /// Otherwise, only ExecuteStart and ExecuteFinish will be called
    /// </summary>
    public virtual bool usesTime => true;

    public List<IActionRule> modifiedBy = new List<IActionRule>();

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