using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    /// <summary>
    /// Separate event class equivalent to UnityEvent in case serialization is 
    /// required.
    /// </summary>
    public class Event : UnityEvent { }

    public enum EventState
    {
        StartTurn,
        EndTurn,
        PostEndTurn,
        PreExecute,
        Execute,
        PostExecute,
    }


    public const float TimePerAction = 1.0f;


    //public List<Rule> rules = new List<Rule>();
    //public Dictionary<string, Rule> namedRules = new Dictionary<string, Rule>();


    public EventState lastBoardEvent { get; private set; }
    public float? endTurnTime { get; private set; } = null;
    public float? timeSinceEndTurn
    {
        get
        {
            return endTurnTime == null ? null : Time.time - endTurnTime;
        }
    }

    public float? actionsSinceEndTurn
    {
        get
        {
            return timeSinceEndTurn == null ? null : timeSinceEndTurn / TimePerAction;
        }
    }

    /// <summary>
    /// Event raised after the execute phase has passed 
    /// maxActions actions (ie. actionsSinceEndTurn > maxActions). 
    /// Used for logic at the start of the turn (eg. collisions)
    /// </summary>
    public Event StartTurnEvent = new Event();

    /// <summary>
    /// Event raised after Board.EndTurn() was called. 
    /// Used for setting up actions.
    /// </summary>
    /// <see cref="EndTurn"/>
    public Event EndTurnEvent = new Event();

    /// <summary>
    /// Event raised immediately after EndTurnEvent
    /// </summary>
    public Event PostEndTurnEvent = new Event();


    /// <summary>
    /// Event raised immediately before PostEndTurnEvent. 
    /// In this time, BoardObjects should perform rule checks on actions.
    /// </summary>
    public Event PreExecuteEvent = new Event();


    /// <summary>
    /// Event raised immediately after PostEndTurnEvent. 
    /// In this time, BoardObjects should execute their actions. 
    /// The phase lasts maxActions * TimePerAction seconds.
    /// </summary>
    public Event ExecuteEvent = new Event();

    /// <summary>
    /// Event raised immediately after the execution phase (before StartTurnEvent). 
    /// In this time, BoardObjects should execute their actions. 
    /// The phase lasts maxActions * TimePerAction seconds.
    /// </summary>
    public Event PostExecuteEvent = new Event();


    public List<IActionRule> actionRules = new List<IActionRule>();


    private int maxActions = 0;

    private void Start()
    {
        StartTurnEvent.AddListener(this.OnStartTurn);
    }


    /// <summary>
    /// Goes from state StartTurn up to Execute
    /// </summary>
    public void EndTurn()
    {
        lastBoardEvent = EventState.EndTurn;
        EndTurnEvent.Invoke();

        lastBoardEvent = EventState.PostEndTurn;
        PostEndTurnEvent.Invoke();

        endTurnTime = Time.time;

        lastBoardEvent = EventState.PreExecute;
        PreExecuteEvent.Invoke();

        lastBoardEvent = EventState.Execute;
        ExecuteEvent.Invoke();

        StartCoroutine(EndTurnCounter(TimePerAction * maxActions));
    }


    /// <summary>
    /// Sets the max actions for the next turn. This will linearly increase how long the endphase will be.
    /// </summary>
    /// <param name="nActions">Max number of actions taken at end of turn</param>
    public void SetMaxActions(int nActions)
    {
        maxActions = nActions > maxActions ? nActions : maxActions;
    }

    public BoardAction ApplyRules(BoardObject boardObject, BoardAction boardAction)
    {
        var currentAction = boardAction;
        foreach (var rule in actionRules)
        {
            var newAction = rule.Execute(currentAction);
            if (!ReferenceEquals(newAction, currentAction))
            {
                newAction.modifiedBy = currentAction.modifiedBy;
                newAction.modifiedBy.Add(rule);
            }
            currentAction = newAction;
        }
        return currentAction;
    }


    /// <summary>
    /// Helper function to broadcast "OnStartTurn" after endphase duration.
    /// </summary>
    /// <param name="duration">Duration of endphase</param>
    /// <returns>generator for coroutine</returns>
    private IEnumerator EndTurnCounter(float duration)
    {
        yield return new WaitForSeconds(duration);
        lastBoardEvent = EventState.PostExecute;
        PostExecuteEvent.Invoke();

        lastBoardEvent = EventState.StartTurn;
        StartTurnEvent.Invoke();
        endTurnTime = null;
    }

    /// <summary>
    /// Receiver to OnStartTurn message. Resets max actions for the next turn.
    /// </summary>
    private void OnStartTurn()
    {
        maxActions = 0;
    }
}
