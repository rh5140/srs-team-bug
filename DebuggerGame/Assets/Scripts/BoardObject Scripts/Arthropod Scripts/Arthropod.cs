using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Arthropod : BoardObject
{
    public bool isCaught { get; private set; } = false;
    public bool rulesEnabled = true;

    protected virtual bool disableOnCatch => true;
    protected virtual bool winOnCatch => true;

    public List<IActionRule> rules { get; protected set; } = new List<IActionRule>();

    private int winConditionIndex;

    protected override void Start()
    {
        base.Start();

        if (winOnCatch)
        {
            winConditionIndex = board.AllocateWinCondition();
        }
    }

    public virtual void Catch()
    {
        isCaught = true;
        if (winOnCatch)
        {
            board.SetWinCondition(winConditionIndex, true);
        }
        
        if (disableOnCatch)
        {
            gameObject.SetActive(false);
        }
    }



    protected void AddActionRule(IActionRule rule)
    {
        rules.Add(rule);
        board.actionRules.Add(rule);
    }
}