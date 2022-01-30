using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBug : Bug
{
    protected override void Start()
    {
        base.Start();
        board.rules.Add(
            Rule.Builder(this)
                .ForAll<Player>()
                .DisableMovementInDirection(Vector2.right)
                .WhileNotCaptured()
                .Build()
        );
    }
}
