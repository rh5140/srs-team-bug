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


    
    public Queue<BoardAction> actions = new Queue<BoardAction>();

    protected int? actionsLeftIndex = null;

    /// <summary>
    /// When in execution phase, the action that is currently being executed
    /// </summary>
    protected BoardAction executingAction = null;

    /// <summary>
    /// The index of the currently executing action to be used to offset execution progress
    /// </summary>
    /// <see cref="executingActionProgress"/>
    protected int? executingActionOffset = null;

    /// <summary>
    /// The progress from 0 to 1 of the currently executing action.
    /// Uses `Board.actionsSinceEndTurn' and offsets it by executingActionIndex
    /// </summary>
    /// <see cref="Board.actionsSinceEndTurn"/>
    /// <see cref="executingActionOffset"/>
    protected float? executingActionProgress
    {
        get { 
            if (board.actionsSinceEndTurn != null && executingActionOffset != null)
            {
                // This line makes more sense in terms of time
                // If we just started executing the second action (index = 1) and it takes 1 second to
                // execute an action, then it should be offset by 1.
                // If we are executing the third action (index = 2), then it should be offset by 2, and so on.
                // So the pattern is offset by the action index
                return Mathf.Clamp01(board.actionsSinceEndTurn.Value - executingActionOffset.Value);
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
            if (board.actionsSinceEndTurn != null && executingActionOffset != null)
            {
                return board.actionsSinceEndTurn.Value - executingActionOffset.Value > 1f;
            }
            else
            {
                return null;
            }
        }
    }


    public void AddActionMidExecution(BoardAction action, int? actionOffset)
    {
        actions.Enqueue(action);
        if(board.lastBoardEvent == Board.EventState.PlayerExecute)
        {
            if(actions.Count == 1)
            {
                executingActionOffset = actionOffset;
                board.SetActionsLeft(this, 1);
            }
        }
    }

    public BoardAction PeekNextBoardAction()
    {
        if(executingAction != null)
        {
            return executingAction;
        }
        else if(actions.Count > 0)
        {
            return actions.Peek();
        }
        else
        {
            return null;
        }
    }

    public Vector2Int PeekNextWeakCoordinate()
    {
        if(!(PeekNextBoardAction() is MovementAction movementAction))
        {
            return coordinate;
        }
        else
        {
            return coordinate + movementAction.direction;
        }
    }


    private void Awake()
    {
        // TODO: Sanitize position to be on the grid?
        // TODO: If there is an offset from the grid, implement for coordinate
        coordinate = new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }
    protected virtual void Start()
    {
        board = Board.instance;

        // Add handlers
        // In future, handlers may be added in the implementation
        // so that empty handlers won't bloat the event, but
        // there shouldb't be a significant performance gain so it's
        // not necessary
        board.StartTurnEvent.AddListener(OnStartTurn);
        board.EndTurnEvent.AddListener(OnEndTurn);
        board.PostEndTurnEvent.AddListener(OnPostEndTurn);

        board.PrePlayerExecuteEvent.AddListener(OnPrePlayerExecute);
        board.PlayerExecuteEvent.AddListener(OnPlayerExecute);
        board.PostPlayerExecuteEvent.AddListener(OnPostPlayerExecute);

        board.PreArthropodExecuteEvent.AddListener(OnPreArthropodExecute);
        board.ArthropodExecuteEvent.AddListener(OnArthropodExecute);
        board.PostArthropodExecuteEvent.AddListener(OnPostArthropodExecute);

        board.EndLevelEvent.AddListener(OnEndLevel);
    }


    protected void OnDestroy()
    {
        RemoveListeners();
    }

    //removes listeners for the functions of this boardobject
    public void RemoveListeners()
    {
        board.StartTurnEvent.RemoveListener(OnStartTurn);
        board.EndTurnEvent.RemoveListener(OnEndTurn);
        board.PostEndTurnEvent.RemoveListener(OnPostEndTurn);

        board.PrePlayerExecuteEvent.RemoveListener(OnPrePlayerExecute);
        board.PlayerExecuteEvent.RemoveListener(OnPlayerExecute);
        board.PostPlayerExecuteEvent.RemoveListener(OnPostPlayerExecute);

        board.PreArthropodExecuteEvent.RemoveListener(OnPreArthropodExecute);
        board.ArthropodExecuteEvent.RemoveListener(OnArthropodExecute);
        board.PostArthropodExecuteEvent.RemoveListener(OnPostArthropodExecute);

        board.EndLevelEvent.RemoveListener(OnEndLevel);
    }

    protected virtual void Update()
    {
        if(board.lastBoardEvent == Board.EventState.PlayerExecute || board.lastBoardEvent == Board.EventState.ArthropodExecute)
        {
            // Execute all the actions in the queue 
            while (executingAction == null && actions.Count > 0)
            {
                // If the previous action finished/we have not started executing, get new action if available
                // and increase the index by 1 (or set to 0 if not previously set)
                executingAction = actions.Dequeue();
                if (!board.FilterStateDependentAction(this, executingAction, executingActionOffset))
                {
                    // state dependent filter disallowed action
                    executingAction = null;
                    board.SetActionsLeft(this, actions.Count);
                }
                else
                {
                    executingAction.ExecuteStart();
                    if (executingAction.usesTime)
                    {
                        executingActionOffset = executingActionOffset + 1 ?? 0; // either increment if nonnull or set to 0
                    }
                    else
                    {
                        executingAction.ExecuteFinish();
                        executingAction = null;
                        board.SetActionsLeft(this, actions.Count);
                    }
                }
                
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
                board.SetActionsLeft(this, actions.Count);
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
    /// Handler for Board.PreExecuteEvent
    /// </summary>
    /// <see cref="Board.PreExecuteEvent"/>
    virtual protected void OnPrePlayerExecute()
    {
        int actionsLeft = 0;
        for (int i = 0; i < actions.Count; i++)
        {
            BoardAction action = board.ApplyRules(this, actions.Dequeue());
            actions.Enqueue(action);
            // If the action uses a turn, then add 1 to max actions
            actionsLeft += action.usesTime ? 1 : 0;
        }
        actionsLeftIndex = board.AllocateActionsLeft(this);
        board.SetActionsLeft(this, actionsLeft);
        // See BoardObject.Update for continuation of the logic
    }

    /// <summary>
    /// Handler for Board.ExecuteEvent
    /// </summary>
    /// <see cref="Board.ExecuteEvent"/>
    protected virtual void OnPlayerExecute()
    { }


    /// <summary>
    /// Handler for Board.PostExecuteEvent
    /// </summary>
    /// <see cref="Board.PostExecuteEvent"/>
    protected virtual void OnPostPlayerExecute()
    {
        if (executingAction != null)
        {
            executingAction?.ExecuteUpdate(executingActionProgress.Value);
            executingAction?.ExecuteFinish();
            executingAction = null;
        }
        executingActionOffset = null;
        actionsLeftIndex = null;
    }
 
    virtual protected void OnPreArthropodExecute()
    {

        int actionsLeft = 0;
        for (int i = 0; i < actions.Count; i++)
        {
            BoardAction action = board.ApplyRules(this, actions.Dequeue());
            actions.Enqueue(action);
            // If the action uses a turn, then add 1 to max actions
            actionsLeft += action.usesTime ? 1 : 0;
        }
        actionsLeftIndex = board.AllocateActionsLeft(this);
        board.SetActionsLeft(this, actionsLeft);
        // See BoardObject.Update for continuation of the logic
    }

    protected virtual void OnArthropodExecute()
    { }


    protected virtual void OnPostArthropodExecute()
    {
        if (executingAction != null)
        {
            executingAction?.ExecuteUpdate(executingActionProgress.Value);
            executingAction?.ExecuteFinish();
            executingAction = null;
        }
        executingActionOffset = null;
        actionsLeftIndex = null;
    }


    /// <summary>
    /// Method to be called when the level finishes
    /// </summary>
    /// <see cref="Board.ExecuteEvent"/>
    protected virtual void OnEndLevel()
    { }
}