using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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


    public static Board instance = null;


    public const float TimePerAction = 0.2f;

    //Bounds
    public int width = 5;
    public int height = 5;
    public bool boundsEnabled = true;
    BoardAction BoundsMap(BoardAction action)
    {
        return new NullAction(action.boardObject);
    }

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

    /// <summary>
    /// Event raised immediately after the execution phase (before StartTurnEvent). 
    /// In this time, BoardObjects should execute their actions. 
    /// The phase lasts maxActions * TimePerAction seconds.
    /// </summary>
    public Event WinEvent = new Event();


    public List<IActionRule> actionRules = new List<IActionRule>();
    public List<IActionRule> postActionRules = new List<IActionRule>();

    public List<BoardObject> boardObjects;

    private int maxActions = 0;


    private Coroutine restartCoroutine = null;
    public bool? restartReleasedSinceStart = null;
    public bool restarting = false;
    private float restartDuration = 2f;
    private float? restartFinishTime = null;
    public float? restartingProgressLeft => (restartFinishTime is float t) ? (float?)((t - Time.time)/restartDuration) : null;


    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Multiple boards");
        }
        else
        {
            instance = this;
        }
    }

    void OnDestroy()
    {
        StartTurnEvent.RemoveAllListeners();
        EndTurnEvent.RemoveAllListeners();
        PostEndTurnEvent.RemoveAllListeners();
        PreExecuteEvent.RemoveAllListeners();
        ExecuteEvent.RemoveAllListeners();
        PostExecuteEvent.RemoveAllListeners();
        WinEvent.RemoveAllListeners();

        if (this == instance)
        {
            instance = null;
        }
    }

    bool BoardObjectAtCoordIsCollidable(Vector2Int coordinate)
    {
        foreach(var boardObject in boardObjects)
        {
            if(boardObject.coordinate == coordinate && boardObject is CollidableObject)
            {
                return true;
            }
        }
        return false;
    }

    private void Start()
    {
        boardObjects = new List<BoardObject>(GetComponentsInChildren<BoardObject>());

        StartTurnEvent.AddListener(this.OnStartTurn);
        postActionRules.Add(
            new EFMActionRule(
                null,
                this,
                enableConditions: new List<EFMActionRule.EnableCondition> {
                    (BoardObject creator, Board board)
                        => board != null && boundsEnabled
                },
                filter: (BoardAction action) =>
                    action.boardObject is Player
                    && action is MovementAction movementAction
                    && (action.boardObject.coordinate.x + movementAction.direction.x < 0 ||
                        action.boardObject.coordinate.x + movementAction.direction.x >= width ||
                        action.boardObject.coordinate.y + movementAction.direction.y < 0 ||
                        action.boardObject.coordinate.y + movementAction.direction.y >= height ||
                        BoardObjectAtCoordIsCollidable(action.boardObject.coordinate + movementAction.direction)
                    ),
                map: BoundsMap
            )
        );
    }

    private void Update()
    {
        if (Input.GetButtonDown("Restart") && !restarting)
        {
            restarting = true;
            restartCoroutine = StartCoroutine(RestartCoroutine(seconds: restartDuration));
        }
        else if (Input.GetButtonUp("Restart") && restarting)
        {
            if (restartCoroutine != null)
            {
                restarting = false;
                StopCoroutine(restartCoroutine);
                restartCoroutine = null;
            }
        }
    }

    IEnumerator RestartCoroutine(float seconds)
    {
        restartFinishTime = Time.time + seconds;
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        foreach (var rule in postActionRules)
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

    private List<bool> winConditions = new List<bool>();

    public bool gameWon = false;

    // add a bool to winConditions and return its index
    public int AllocateWinCondition()
    {
        winConditions.Add(false);
        return winConditions.Count - 1;
    }

    // set the win condition and test for whether all are met
    // if all are met, then gg
    public void SetWinCondition(int index, bool value)
    {
        winConditions[index] = value;
        gameWon = !winConditions.Contains(false);
        if (gameWon)
        {
            WinEvent.Invoke();
        }
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
