using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionlessBoardObject : BoardObject
{
    protected override void Update()
    { }

    protected override void OnPrePlayerExecute()
    { }

    protected override void OnPostPlayerExecute()
    { }

    protected override void OnPreArthropodExecute()
    { }

    protected override void OnPostArthropodExecute()
    { }
}
