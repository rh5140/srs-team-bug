using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Arthropod : BoardObject
{
    public bool isCaught { get; private set; } = false;
    public bool rulesEnabled = true;


    public List<IActionRule> rules { get; protected set; } = new List<IActionRule>();

    public void Catch()
    {
        isCaught = true;
        Debug.Log("Bug Caught, Bugs remaining: " + this.board.GetNumBugs());
    }



    protected void AddActionRule(IActionRule rule)
    {
        rules.Add(rule);
        board.actionRules.Add(rule);
    }
}