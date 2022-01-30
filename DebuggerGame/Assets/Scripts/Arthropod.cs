using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Arthropod : BoardObject
{
    private bool isCaught = false;


    public void caughtByPlayer()
    {
        throw new System.NotImplementedException();
    }

    protected virtual void EndTurn()
    {
        throw new System.NotImplementedException();
    }

}