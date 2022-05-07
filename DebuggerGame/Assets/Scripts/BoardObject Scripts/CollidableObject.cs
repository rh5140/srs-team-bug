using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableObject : BoardObject
{
    public bool bugsCanPass = false;

    public bool BugsCanPass() {
        return bugsCanPass;
    }
}
