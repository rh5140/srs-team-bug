using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Bug : MonoBehaviour
{
    private bool isCaught = false;


    public void caughtByPlayer()
    {
        throw new System.NotImplementedException();
    }

    virtual void EndTurn()
    {
        throw new System.NotImplementedException();
    }

}