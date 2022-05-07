using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Arthropod : BoardObject
{
    public bool isCaught { get; private set; } = false;
    public bool rulesEnabled = true;
    

    public List<IActionRule> rules { get; protected set; } = new List<IActionRule>();


    public virtual void Catch(GameObject player)
    {
        isCaught = true;
        rulesEnabled = false;
        transform.SetParent(player.transform, true);
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        player.GetComponent<Player>().setArthropod(this.GetComponent<Arthropod>());
    }

    public virtual void Release(GameObject player)
    {
        isCaught = false;
        rulesEnabled = true;
        transform.SetParent(board.transform, true);
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        player.GetComponent<Player>().setArthropod(null);
    }



    protected void AddActionRule(IActionRule rule)
    {
        rules.Add(rule);
        board.actionRules.Add(rule);
    }

    protected override void OnStartTurn()
    {
        base.OnStartTurn();
        this.coordinate = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Debug.Log(this.coordinate);
    }
}