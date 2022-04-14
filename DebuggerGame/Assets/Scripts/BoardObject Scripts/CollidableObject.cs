using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableObject : BoardObject
{
    protected bool bugsCanPass = true;

    public bool BugsCanPass() {
        return bugsCanPass;
    }
}
