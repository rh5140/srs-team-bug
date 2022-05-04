using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Arthropod : BoardObject
{
    public bool isCaught { get; private set; } = false;
    public bool rulesEnabled = true;

    public List<IActionRule> rules { get; protected set; } = new List<IActionRule>();

    
    private int winConIndex;
    protected override void Start()
    {
        base.Start();   
        winConIndex = board.AllocateWinCondition();
    }

    public virtual void Catch(GameObject player)
    {
        isCaught = true;
        rulesEnabled = false;
        transform.SetParent(player.transform, true);
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        player.GetComponent<Player>().setArthropod(this.GetComponent<Arthropod>());
        board.BugCountDecrement();
    }

    public virtual void Release(GameObject player)
    {
        isCaught = false;
        rulesEnabled = true;
        transform.SetParent(board.transform, true);
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        player.GetComponent<Player>().setArthropod(null);
        board.BugCountIncrement();
    }

    public virtual void Swallow(GameObject player)
    {
        player.GetComponent<Player>().setArthropod(null);
        board.SetWinCondition(winConIndex, true);
        board.BugCountDecrement();
        board.boardObjects.Remove(this.gameObject.GetComponent<BoardObject>());
        this.gameObject.SetActive(false);
        Debug.Log("Swallowed");
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
        //Debug.Log(this.coordinate);
    }
}