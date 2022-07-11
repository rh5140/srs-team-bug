using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arthropod : BoardObject
{
    /* ARTHROPOD BEHAVIORS */

    public MovingArthropodBehavior movingArthropodBehavior = new MovingArthropodBehavior();
    public RestrictMovementArthropodBehavior restrictMovementArthropodBehavior = new RestrictMovementArthropodBehavior();

    /* END ARTHROPOD BEHAVIOR */

    private List<ArthropodBehavior> enabledArthropodBehaviors = new List<ArthropodBehavior>();

    public static bool DefaultArthropodEnableCondition(BoardObject creator, Board board)
    {
        return creator is Arthropod arthropod && arthropod.rulesEnabled;
    }


    public bool isCaught { get; private set; } = false;
    public bool rulesEnabled = true;

    public List<IActionRule> rules { get; protected set; } = new List<IActionRule>();

    
    private int winConIndex;
    protected override void Start()
    {
        base.Start();
        foreach(var field in typeof(Arthropod).GetFields())
        {
            if (field.FieldType.IsSubclassOf(typeof(ArthropodBehavior)))
            {
                var behavior = ((ArthropodBehavior)field.GetValue(this));
                if (behavior.enabled)
                {
                    enabledArthropodBehaviors.Add(behavior);
                }
            }
        }

        winConIndex = board.AllocateWinCondition();

        foreach(var behavior in enabledArthropodBehaviors)
        {
            behavior.Start(this);
        }
    }

    public virtual void Catch(GameObject player)
    {
        isCaught = true;
        //rulesEnabled = false;
        transform.SetParent(player.transform, true);
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        player.GetComponent<Player>().setArthropod(this);
        board.BugsCaughtIncrement();
    }

    public virtual void Release(GameObject player)
    {
        isCaught = false;
        //rulesEnabled = true;
        transform.SetParent(board.transform, true);
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        player.GetComponent<Player>().setArthropod(null);
        board.BugsCaughtDecrement();
    }

    public virtual void Swallow(GameObject player)
    {
        player.GetComponent<Player>().setArthropod(null);
        rulesEnabled = false;
        board.SetWinCondition(winConIndex, true);
        board.DeallocateActionsLeft(this);
        board.BugCountDecrement();
        board.boardObjects.Remove(this.gameObject.GetComponent<BoardObject>());
        RemoveListeners();
        this.gameObject.SetActive(false);
        Debug.Log("Swallowed");
    }

    public void AddActionRule(IActionRule rule)
    {
        rules.Add(rule);
        board.actionRules.Add(rule);
    }

    protected override void OnStartPlayerTurn()
    {
        base.OnStartPlayerTurn();
        this.coordinate = new Vector2Int((int)transform.position.x, (int)transform.position.y);

        foreach(var behavior in enabledArthropodBehaviors)
        {
            behavior.OnStartPlayerTurn();
        }
    }

    protected override void OnEndPlayerTurn()
    {
        base.OnEndPlayerTurn();
        foreach (var behavior in enabledArthropodBehaviors)
        {
            behavior.OnEndPlayerTurn();
        }
    }

    protected override void OnPostPlayerEndTurn()
    {
        base.OnPostPlayerEndTurn();
        foreach (var behavior in enabledArthropodBehaviors)
        {
            behavior.OnPostPlayerEndTurn();
        }
    }

    protected override void OnStartArthropodTurn()
    {
        base.OnStartArthropodTurn();
        foreach (var behavior in enabledArthropodBehaviors)
        {
            behavior.OnStartArthropodTurn();
        }
    }

    protected override void OnEndArthropodTurn()
    {
        base.OnEndArthropodTurn();
        foreach (var behavior in enabledArthropodBehaviors)
        {
            behavior.OnEndArthropodTurn();
        }
    }
}