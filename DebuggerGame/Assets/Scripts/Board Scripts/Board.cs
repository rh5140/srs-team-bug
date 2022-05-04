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

    //Felix: Temporary implementation for bug counting (done with permissions from HiccupHan)
    private int numBugs; //Number of bugs currently left in the stage

    public List<BoardObject> boardObjects;

    public void BugCountIncrement()
    {
        numBugs++;
    }
    public void BugCountDecrement()
    {
        numBugs--;
    }
    public int GetNumBugs()
    {
        return numBugs;
    }


    public static Board instance { get; private set; } = null;


    public const float TimePerAction = 1.0f;

    //Bounds
    public int width = 5;
    public int height = 5;
    public bool boundsEnabled = true;
    public bool collidablesEnabled = true;

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

    public Dictionary<Vector2Int, CollidableObject> collidableCoordinates = new Dictionary<Vector2Int, CollidableObject>();

    private int maxActions = 0;

    //Determines if a BoardObject can enter a coordinate
    public bool CanEnterCoordinate(BoardObject boardObject, Vector2Int coordinate) {
        bool canPass = !collidableCoordinates.ContainsKey(coordinate)
                    || boardObject is Arthropod && collidableCoordinates[coordinate].BugsCanPass();
        bool inBounds = !(coordinate.x < 0 || coordinate.x >= width || coordinate.y < 0 || coordinate.y >= height);
        return canPass && inBounds;
    }

    private void OnEnable()
    {
        // Singleton pattern
        Debug.AssertFormat(Board.instance == null, "Multiple instances of Board is not supported. Last instance: {0}", Board.instance);
        Board.instance = this;
    }

    private void Start()
    {
        StartTurnEvent.AddListener(this.OnStartTurn);

        boardObjects = new List<BoardObject>(GetComponentsInChildren<BoardObject>());

        //Bug counting initialization
        numBugs = CountBoardObjectsOfType<Arthropod>();

        //Initialize collidables list
        foreach(CollidableObject collidable in GetBoardObjectsOfType<CollidableObject>()) {
            collidableCoordinates.Add(collidable.coordinate, collidable);
        }

        actionRules.Add(
            new EFMActionRule(
                null,
                this,
                enableConditions: new List<EFMActionRule.EnableCondition> {
                    (BoardObject creator, Board board)
                        => board != null && boundsEnabled
                },
                filter: (BoardAction action) =>
                    //action.boardObject is Player
                     action is MovementAction movementAction
                    && (action.boardObject.coordinate.x + movementAction.direction.x < 0 ||
                        action.boardObject.coordinate.x + movementAction.direction.x >= width ||
                        action.boardObject.coordinate.y + movementAction.direction.y < 0 ||
                        action.boardObject.coordinate.y + movementAction.direction.y >= height),
                map: (BoardAction action) => {return new NullAction(action.boardObject);}
            )
        );

        actionRules.Add(
            new EFMActionRule(
                null,
                this,
                enableConditions: new List<EFMActionRule.EnableCondition> {
                    (BoardObject creator, Board board)
                        => board != null && collidablesEnabled
                },
                filter: (BoardAction action) =>
                    action is MovementAction movementAction
                    && !CanEnterCoordinate(action.boardObject, 
                        new Vector2Int(
                            action.boardObject.coordinate.x + movementAction.direction.x,
                            action.boardObject.coordinate.y + movementAction.direction.y
                    )),
                map: (BoardAction action) => {return new NullAction(action.boardObject);}
            )
        );
    }

    private void OnDisable()
    {
        Board.instance = null;
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

        StartCoroutine(EndTurnCounter(TimePerAction * (maxActions == 0 ? 0.2f : maxActions)));
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
            Debug.Log("You won woo.");
        }
    }


    #region BoardObjects helper functions
    // IEnumerable for lazy evaluation
    public IEnumerable<T> GetBoardObjectsOfType<T>() where T: BoardObject {
        foreach(BoardObject boardObject in boardObjects)
        {
            if (boardObject is T found)
            {
                yield return found;
            }
        }
    }

    public T GetBoardObjectOfType<T>() where T : BoardObject
    {
        return boardObjects
            .Find(boardObject => boardObject is T) as T;
    }

    public int CountBoardObjectsOfType<T>() where T : BoardObject
    {
        int count = 0;
        foreach(BoardObject boardObject in boardObjects)
        {
            count += boardObject is T ? 1 : 0;
        }
        return count;
    }


    // IEnumerable for lazy evaluation
    public IEnumerable<BoardObject> GetBoardObjectsAtCoordinate(Vector2Int coordinate)
    {
        foreach (BoardObject boardObject in boardObjects)
        {
            if (boardObject.coordinate == coordinate)
            {
                yield return boardObject;
            }
        }
    }

    
    public IEnumerable<BoardObject> GetBoardObjectsAtCoordinate(int x, int y)
    {
        return GetBoardObjectsAtCoordinate(new Vector2Int(x, y));
    }

    public BoardObject GetBoardObjectAtCoordinate(Vector2Int coordinate)
    {
        return boardObjects
            .Find(boardObject => boardObject.coordinate == coordinate);
    }

    public BoardObject GetBoardObjectAtCoordinate(int x, int y)
    {
        return GetBoardObjectAtCoordinate(new Vector2Int(x, y));
    }


    public int CountBoardObjectsAtCoordinate(Vector2Int coordinate)
    {
        int count = 0;
        foreach(BoardObject boardObject in boardObjects)
        {
            count += boardObject.coordinate == coordinate ? 1 : 0;
        }
        return count;
    }


    public int CountBoardObjectsAtCoordinate(int x, int y)
    {
        return CountBoardObjectsAtCoordinate(new Vector2Int(x, y));
    }

    #endregion


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
