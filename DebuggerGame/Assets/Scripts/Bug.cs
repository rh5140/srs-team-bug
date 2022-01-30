using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Bug : BoardObject
{
    public bool isCaught { get; private set; } = false;

    public void Catch()
    {
        isCaught = true;
    }
}