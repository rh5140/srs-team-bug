using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Board : MonoBehaviour
{
    public enum State
    {
        StartTurn,
        EndTurn,
        PostEndTurn,
        Execute,
        PostExecute,
    }


    public const float TimePerAction = 1.0f;


    public State state { get; private set; }
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


    private int maxActions = 0;

    // NOTE: `Action<Arg1, Arg2, ...>` = `delegate void Action(Arg1, Arg2, ...)`
    // Action has nothing to do with BoardAction

    /// <summary>
    /// Event raised after the execute phase has passed 
    /// maxActions actions (ie. actionsSinceEndTurn > maxActions). 
    /// Used for logic at the start of the turn (eg. collisions)
    /// </summary>
    public event Action StartTurnEvent;

    /// <summary>
    /// Event raised after Board.EndTurn() was called. 
    /// Used for setting up actions.
    /// </summary>
    /// <see cref="EndTurn"/>
    public event Action EndTurnEvent;

    /// <summary>
    /// Event raised immediately after EndTurnEvent
    /// </summary>
    public event Action PostEndTurnEvent;

    /// <summary>
    /// Event raised immediately after PostEndTurnEvent. 
    /// In this time, BoardObjects should execute their actions. 
    /// The phase lasts maxActions * TimePerAction seconds.
    /// </summary>
    public event Action ExecuteEvent;

    /// <summary>
    /// Event raised immediately after the execution phase (before StartTurnEvent). 
    /// In this time, BoardObjects should execute their actions. 
    /// The phase lasts maxActions * TimePerAction seconds.
    /// </summary>
    public event Action PostExecuteEvent;


    private void Start()
    {
        StartTurnEvent += this.OnStartTurn;
    }


    /// <summary>
    /// Goes from state StartTurn up to Execute
    /// </summary>
    public void EndTurn()
    {
        state = State.EndTurn;
        EndTurnEvent?.Invoke();

        state = State.PostEndTurn;
        PostEndTurnEvent?.Invoke();

        state = State.Execute;
        ExecuteEvent?.Invoke();
        endTurnTime = Time.time;

        //BroadcastMessage("OnEndTurn", null, SendMessageOptions.DontRequireReceiver);
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


    /// <summary>
    /// Helper function to broadcast "OnStartTurn" after endphase duration.
    /// </summary>
    /// <param name="duration">Duration of endphase</param>
    /// <returns>generator for coroutine</returns>
    private IEnumerator EndTurnCounter(float duration)
    {
        yield return new WaitForSeconds(duration);
        state = State.PostExecute;
        PostExecuteEvent?.Invoke();

        state = State.StartTurn;
        StartTurnEvent?.Invoke();
        endTurnTime = -1f;
        //BroadcastMessage("OnStartTurn", null, SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Receiver to OnStartTurn message. Resets max actions for the next turn.
    /// </summary>
    void OnStartTurn()
    {
        maxActions = 0;
    }
}
