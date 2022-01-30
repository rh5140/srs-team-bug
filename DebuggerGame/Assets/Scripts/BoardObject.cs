using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BoardObject : MonoBehaviour
{
    public Board board { get; private set; }

    /// <summary>
    /// The current *actual* position.
    /// So if we move, the coordinate will immediately change, while 
    /// the position may slowly change.
    /// </summary>
    [System.NonSerialized]
    public Vector2Int coordinate;


    
    protected Queue<BoardAction> actions = new Queue<BoardAction>();
    /// <summary>
    /// When in execution phase, the action that is currently being executed
    /// </summary>
    protected BoardAction executingAction = null;

    /// <summary>
    /// The index of the currently executing action to be used to offset execution progress
    /// </summary>
    /// <see cref="executingActionProgress"/>
    protected int? executingActionIndex = null;

    /// <summary>
    /// The progress from 0 to 1 of the currently executing action.
    /// Uses `Board.actionsSinceEndTurn' and offsets it by executingActionIndex
    /// </summary>
    /// <see cref="Board.actionsSinceEndTurn"/>
    /// <see cref="executingActionIndex"/>
    protected float? executingActionProgress
    {
        get { 
            if (board.actionsSinceEndTurn != null && executingActionIndex != null)
            {
                // This line makes more sense in terms of time
                // If we just started executing the second action (index = 1) and it takes 1 second to
                // execute an action, then it should be offset by 1.
                // If we are executing the third action (index = 2), then it should be offset by 2, and so on.
                // So the pattern is offset by the action index
                return Mathf.Clamp01(board.actionsSinceEndTurn.Value - executingActionIndex.Value);
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Whether the currently executing action's progress is > 100%
    /// </summary>
    /// <see cref="executingActionProgress"/>
    protected bool? executingActionComplete
    {
        get
        {
            if (board.actionsSinceEndTurn != null && executingActionIndex != null)
            {
                return board.actionsSinceEndTurn.Value - executingActionIndex.Value > 1f;
            }
            else
            {
                return null;
            }
        }
    }

    protected virtual void Start()
    {
        // TODO: Sanitize position to be on the grid?
        // TODO: If there is an offset from the grid, implement for coordinate
        coordinate = new Vector2Int((int)transform.position.x, (int)transform.position.y);

        board = GetComponentInParent<Board>();

        // Add handlers
        // In future, handlers may be added in the implementation
        // so that empty handlers won't bloat the event, but
        // there shouldb't be a significant performance gain so it's
        // not necessary
        board.StartTurnEvent += OnStartTurn;
        board.EndTurnEvent += OnEndTurn;
        board.PostEndTurnEvent += OnPostEndTurn;
        board.ExecuteEvent += OnExecute;
        board.PostExecuteEvent += OnPostExecute;
    }

    
    protected virtual void Update()
    {
        if(board.state == Board.State.Execute)
        {
            // Execute all the actions in the queue 
            if (executingAction == null && actions.Count > 0)
            {
                // If the previous action finished/we have not started executing, get new action if available
                // and increase the index by 1 (or set to 0 if not previously set)
                executingAction = actions.Dequeue();
                executingAction.ExecuteStart();
                executingActionIndex = executingActionIndex + 1 ?? 0; // either increment if nonnull or set to 0
            }


            // Every time executingAction is nonnull, executingActionProgress is nonnull
            // so executingActionProgress.Value
            executingAction?.ExecuteUpdate(executingActionProgress.Value);


            if (executingActionComplete ?? false)
            {
                // (?? is used to convert null to false)
                // If it has been at least 1.0 actions since when the action started, the action finished
                executingAction?.ExecuteFinish();
                executingAction = null;
            }
        }
    }


    /// <summary>
    /// Handler for Board.StartTurnEvent
    /// </summary>
    /// <see cref="Board.StartTurnEvent"/>
    protected virtual void OnStartTurn()
    { }


    /// <summary>
    /// Handler for Board.EndTurnEvent
    /// </summary>
    /// <see cref="Board.EndTurnEvent"/>
    protected virtual void OnEndTurn()
    { }


    /// <summary>
    /// Handler for Board.PostEndTurnEvent
    /// </summary>
    /// <see cref="Board.PostEndTurnEvent"/>
    protected virtual void OnPostEndTurn()
    { }


    /// <summary>
    /// Handler for Board.ExecuteEvent
    /// </summary>
    /// <see cref="Board.ExecuteEvent"/>
    protected virtual void OnExecute()
    {
        board.SetMaxActions(actions.Count);
    }


    /// <summary>
    /// Handler for Board.PostExecuteEvent
    /// </summary>
    /// <see cref="Board.PostExecuteEvent"/>
    protected virtual void OnPostExecute()
    {
        if (executingAction != null)
        {
            executingAction?.ExecuteUpdate(executingActionProgress.Value);
            executingAction?.ExecuteFinish();
            executingAction = null;
        }
        executingActionIndex = null;
    }
}
