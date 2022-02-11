using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullAction : BoardAction
{
    public override bool usesTime => false;

    public NullAction(
        BoardObject actor,
        BoardAction oldAction = null,
        IActionRule modifiedBy = null) : base(actor)
    { }
}
