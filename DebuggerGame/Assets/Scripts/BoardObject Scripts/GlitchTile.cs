using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchTile : CollidableObject
{
    public GlitchTile() {
        bugsCanPass = true;
        pushablesCanPass = true;
    }
}
