using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ArthropodBehavior
{
    public virtual bool UsesRange => false;

    [HideInInspector]
    public bool enabled = false;
    public int range = -1;

    protected Arthropod arthropod;
    public virtual void Start(Arthropod arthropod)
    {
        this.arthropod = arthropod;
    }

    public bool FilterRange(BoardAction action)
    {
        if(range >= 0)
        {
            return action.boardObject.DistanceTo(arthropod) <= range;
        }
        return true;
    }

    public virtual void OnStartPlayerTurn() { }
    public virtual void OnEndPlayerTurn() { }
    public virtual void OnPostPlayerEndTurn() { }

    public virtual void OnStartArthropodTurn() { }
    public virtual void OnEndArthropodTurn() { }
}
