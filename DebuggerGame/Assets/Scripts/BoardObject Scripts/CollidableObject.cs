using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableObject : ActionlessBoardObject
{
    public bool bugsCanPass = false;
    public bool pushablesCanPass = false;

    public bool BugsCanPass() {
        return bugsCanPass;
    }
}
