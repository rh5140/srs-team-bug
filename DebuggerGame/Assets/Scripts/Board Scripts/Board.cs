using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    /// <summary>
    /// Separate event class equivalent to UnityEvent in case serialization is 
    /// required.
    /// </summary>
    public class Event : UnityEvent { }

    public enum EventState
    {
        StartPlayerTurn,
        EndPlayerTurn,
        PostPlayerEndTurn,
        
        PrePlayerExecute,
        PlayerExecute,
        PostPlayerExecute,

        StartArthropodTurn,
        EndArthropodTurn,

        PreArthropodExecute,
        ArthropodExecute,
        PostArthropodExecute,

        EndLevel,
    }

    //Name of level (In the format of 4 characters first two indicating world and last two indicating level) 
    //Example levelName: 0000 (world 0 level 0)
    public string levelName;

    //The levelNames of the levels that are unlocked upon finishing this level
    public List<string> unlockLevels = new List<string>();

    //Felix: Temporary implementation for bug counting (done with permissions from HiccupHan)
    public int numBugs { get; private set; } //Number of bugs currently left in the stage
    public int nBugsCaught { get; private set; }

    public List<BoardObject> boardObjects;

    public Event BugsCaughtChangeEvent = new Event();

    public void BugCountIncrement()
    {
        numBugs++;
    }
    public void BugCountDecrement()
    {
        numBugs--;
    }

    public void BugsCaughtIncrement()
    {
        nBugsCaught++;
        BugsCaughtChangeEvent.Invoke();
    }
    public void BugsCaughtDecrement()
    {
        nBugsCaught--;
        BugsCaughtChangeEvent.Invoke();
    }

    private Collider2D collidableTilemap;
    private Collider2D glitchTilemap;

    public static Board instance { get; private set; } = null;


    public const float TimePerAction = 0.3f;
    public const float EmptyExecutionTime = 0.1f;

    //Bounds
    public int width = 5;
    public int height = 5;
    public bool boundsEnabled = true;
    public bool collidablesEnabled = true;


    #region undo

    private Dictionary<BoardObject, Dictionary<string, object>> currentUndoState = null;
    private Stack<Dictionary<BoardObject, Dictionary<string, object>>> undoStack = new Stack<Dictionary<BoardObject, Dictionary<string, object>>>();

    #endregion

    //public List<Rule> rules = new List<Rule>();
    //public Dictionary<string, Rule> namedRules = new Dictionary<string, Rule>();


    public EventState lastBoardEvent { get; private set; }
    public float? startExecuteTime { get; private set; } = null;
    public float? timeSinceEndTurn
    {
        get
        {
            return startExecuteTime == null ? null : Time.time - startExecuteTime;
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
    /// Invoked after Start has finished
    /// </summary>
    public Event ReadyEvent = new Event();
    public bool ready = false;

    /// <summary>
    /// Event raised after the execute phase has passed 
    /// maxActions actions (ie. actionsSinceEndTurn > maxActions). 
    /// Used for logic at the start of the turn (eg. collisions)
    /// </summary>
    public Event StartPlayerTurnEvent = new Event();

    /// <summary>
    /// Event raised after Board.EndTurn() was called. 
    /// Used for setting up actions.
    /// </summary>
    /// <see cref="EndTurn"/>
    public Event EndPlayerTurnEvent = new Event();

    /// <summary>
    /// Event raised immediately after EndTurnEvent
    /// </summary>
    public Event PostPlayerEndTurnEvent = new Event();


    /// <summary>
    /// Event raised immediately before PostEndTurnEvent. 
    /// In this time, BoardObjects should perform rule checks on actions.
    /// </summary>
    public Event PrePlayerExecuteEvent = new Event();


    /// <summary>
    /// Event raised immediately after PostEndTurnEvent. 
    /// In this time, BoardObjects should execute their actions. 
    /// The phase lasts maxActions * TimePerAction seconds.
    /// </summary>
    public Event PlayerExecuteEvent = new Event();

    /// <summary>
    /// Event raised immediately after the execution phase (before StartTurnEvent). 
    /// In this time, BoardObjects should execute their actions. 
    /// The phase lasts maxActions * TimePerAction seconds.
    /// </summary>
    public Event PostPlayerExecuteEvent = new Event();

    public Event StartArthropodTurnEvent = new Event();
    public Event EndArthropodTurnEvent = new Event();

    public Event PreArthropodExecuteEvent = new Event();
    public Event ArthropodExecuteEvent = new Event();
    public Event PostArthropodExecuteEvent = new Event();

    /// <summary>
    /// Event raised  after the wincondition is satisfied 
    /// and before the return to the map screen
    /// </summary>
    public Event EndLevelEvent = new Event();

    /// <summary>
    /// Action rules take in an action and output a new one
    /// </summary>
    public List<IActionRule> actionRules = new List<IActionRule>();

    /// <summary>
    /// Action filter rules are action rules that only delete/keep actions, not modify
    /// </summary>
    public List<IActionRule> actionFilterRules = new List<IActionRule>();

    /// <summary>
    /// Action filter rules are action rules that only delete/keep actions, not modify
    /// State dependent means it is dependent on a state of the BoardObject
    /// </summary>
    public List<IStateActionRule> stateDependentActionFilterRules = new List<IStateActionRule>();


    public Dictionary<Vector2Int, CollidableObject> collidableCoordinates;

    private Dictionary<BoardObject, int> actionsLeftDict = new Dictionary<BoardObject, int>();

    // Determines if a BoardObject can enter a coordinate
    public bool CanEnterCoordinate(BoardObject boardObject, Vector2Int coordinate)
    {
        bool pushableAtCoord = false;
        if (boardObject is Arthropod) {
            foreach (PushableObject pushable in instance.GetBoardObjectsOfType<PushableObject>()) {
                if (pushable.coordinate == coordinate) pushableAtCoord = true;
            }
        }

        // Bug moving into collidable or glitch
        bool collidableAtCoord = (
                collidableCoordinates.ContainsKey(coordinate)
                && !(boardObject is Arthropod && collidableCoordinates[coordinate].BugsCanPass())
            )
            || (boardObject is Arthropod && pushableAtCoord);

        // Pushable moving into glitch
        bool pushableOnGlitch = (
                collidableCoordinates.ContainsKey(coordinate)
                && boardObject is PushableObject && collidableCoordinates[coordinate].BugsCanPass());

        // Within level bounds
        bool inBounds = !(coordinate.x < 0 || coordinate.x >= width || coordinate.y < 0 || coordinate.y >= height);

        return (!collidableAtCoord || pushableOnGlitch) && inBounds;// !collidableAtCoord;
    }

    private void OnEnable()
    {
        // Singleton pattern
        Debug.AssertFormat(Board.instance == null, "Multiple instances of Board is not supported. Last instance: {0}", Board.instance);
        Board.instance = this;
    }

    private void Start()
    {
        PostPlayerExecuteEvent.AddListener(this.OnPostPlayerExecute);
        StartPlayerTurnEvent.AddListener(this.OnStartPlayerTurn);

        collidableCoordinates = new Dictionary<Vector2Int, CollidableObject>();
        boardObjects = new List<BoardObject>(GetComponentsInChildren<BoardObject>());
       
        //Bug counting initialization
        numBugs = CountBoardObjectsOfType<Arthropod>();
        nBugsCaught = 0;

        // Check if every point within the bounds of the gameboard lies within the collidableTilemap2D bounds. If a point is
        //  within these bounds, add it to collidableCoordinates
        collidableTilemap = GameObject.FindWithTag("Tilemap_Colliders").GetComponent<TilemapCollider2D>();
        if (GameObject.FindWithTag("Tilemap_Glitches") != null)
            glitchTilemap = GameObject.FindWithTag("Tilemap_Glitches").GetComponent<TilemapCollider2D>();
        else glitchTilemap = null;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2Int currentPos = new Vector2Int(i, j);
                if (collidableTilemap.OverlapPoint(currentPos))
                {
                    // GetBoardObjectOfType<CollidableObject>()) is probably not the best way to do this, but it works.
                    //  The CollidableObject.cs script is now only used to ensure this doesn't lead to a NullReferenceExeption
                    collidableCoordinates.Add(currentPos, GetBoardObjectOfType<CollidableObject>());
                }
                else if (glitchTilemap != null && glitchTilemap.OverlapPoint(currentPos))
                {
                    // Note: Cannot have a glitch and collidable tile at the same coordinate! Careful when creating the tilemap!
                    collidableCoordinates.Add(currentPos, GetBoardObjectOfType<GlitchTile>());
                }
            }
        }

        actionFilterRules.Add(
            new EFActionDeleterRule(
                null,
                this,
                null,
                filter: (BoardAction action) =>
                    action is MovementAction movementAction
                    && movementAction.direction.sqrMagnitude == 0
            )
        );

        stateDependentActionFilterRules.Add(
            new EFStateActionDeleterRule(
                null,
                this,
                enableCondition: (BoardObject creator, Board board, int? offset) =>
                    board != null
                    && boundsEnabled,
                filter: (BoardAction action, int? offset) =>
                    action is MovementAction movementAction
                    && (action.boardObject.coordinate.x + movementAction.direction.x < 0 ||
                        action.boardObject.coordinate.x + movementAction.direction.x >= width ||
                        action.boardObject.coordinate.y + movementAction.direction.y < 0 ||
                        action.boardObject.coordinate.y + movementAction.direction.y >= height)
            )
        );

        stateDependentActionFilterRules.Add(
            new EFStateActionDeleterRule(
                null,
                this,
                enableCondition: (BoardObject creator, Board board, int? offset)
                        => board != null && boundsEnabled,
                filter: (BoardAction action, int? offset) =>
                    action.boardObject is Player                                         // Action performed by player
                    && action is MovementAction movementAction                           // Action is movement
                    && GetBoardObjectAtCoordinate(                                       // Target destination has a pushable
                        action.boardObject.coordinate.x + movementAction.direction.x,
                        action.boardObject.coordinate.y + movementAction.direction.y
                    ) is PushableObject pushableObject

                    && CanEnterCoordinate(                                               // Player can move to target destination
                        action.boardObject,
                        action.boardObject.coordinate + movementAction.direction
                    )

                    && !(pushableObject.Push(movementAction.direction, offset))          // Pushable can be pushed
            )
        );

        stateDependentActionFilterRules.Add(
            new EFStateActionDeleterRule(
                null,
                this,
                enableCondition: (BoardObject creator, Board board, int? offset) =>
                    board != null
                    && boundsEnabled,
                filter: (BoardAction action, int? offset) =>
                    action is MovementAction movementAction
                    && !CanEnterCoordinate(
                        action.boardObject, 
                        action.boardObject.coordinate + movementAction.direction
                    )
            )
        );

        ReadyEvent.Invoke();
        ready = true;

        PushUndoStack();
    }

    private void OnDisable()
    {
        Board.instance = null;
    }

    #region undo

    private void PushUndoStack()
    {
        var undoData = new Dictionary<BoardObject, Dictionary<string, object>>();
        foreach (var boardObject in boardObjects)
        {
            undoData[boardObject] = boardObject.SaveState();
        }
        if(currentUndoState != null)
        {
            undoStack.Push(currentUndoState);
        }
        currentUndoState = undoData;
    }

    public void Undo()
    {
        Debug.Assert(lastBoardEvent == EventState.StartPlayerTurn, "Cannot call undo after EndTurn");

        if(undoStack.Count > 0)
        {
            currentUndoState = undoStack.Pop();
            foreach (var boardObject in boardObjects)
            {
                if (currentUndoState.ContainsKey(boardObject))
                {
                    boardObject.LoadState(currentUndoState[boardObject]);
                }
                else
                {
                    boardObject.LoadState(null);
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// Goes from state StartTurn up to Execute
    /// </summary>
    public void EndTurn()
    {
        lastBoardEvent = EventState.EndPlayerTurn;
        EndPlayerTurnEvent.Invoke();

        lastBoardEvent = EventState.PostPlayerEndTurn;
        PostPlayerEndTurnEvent.Invoke();

        startExecuteTime = Time.time;

        lastBoardEvent = EventState.PrePlayerExecute;
        PrePlayerExecuteEvent.Invoke();

        lastBoardEvent = EventState.PlayerExecute;
        PlayerExecuteEvent.Invoke();

        StartCoroutine(EndTurnCounter());
    }


    public void AllocateActionsLeft(BoardObject boardObject)
    {
        actionsLeftDict.Add(boardObject, 0);
    }

    public void DeallocateActionsLeft(BoardObject boardObject)
    {
        actionsLeftDict.Remove(boardObject);
    }

    /// <summary>
    /// Sets the max actions for the next turn. This will linearly increase how long the endphase will be.
    /// </summary>
    /// <param name="nActions">Max number of actions taken at end of turn</param>
    public void SetActionsLeft(BoardObject boardObject, int nActions)
    {
        actionsLeftDict[boardObject] = nActions;
    }

    public bool isInRange(BoardObject boardObject, RangedBug rangedBug) {
        return boardObject.coordinate.x <= rangedBug.coordinate.x + rangedBug.range &&
           boardObject.coordinate.x >= rangedBug.coordinate.x - rangedBug.range &&
           boardObject.coordinate.y <= rangedBug.coordinate.y + rangedBug.range &&
           boardObject.coordinate.y >= rangedBug.coordinate.y - rangedBug.range;
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
        foreach (var rule in actionFilterRules)
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

    public bool FilterStateDependentAction(BoardObject boardObject, BoardAction action, int? actionOffset)
    {
        foreach (var rule in stateDependentActionFilterRules)
        {
            var newAction = rule.Execute(action, actionOffset);
            if (newAction is NullAction)
            {
                return false;
            }
        }
        return true;
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
            EndLevel();
        }
    }

    //Call this method to instantly win
    public void InstantWin()
    {
        EndLevel();
    }

    //Name of the world map scene
    private string mapSceneName = "world_map";

    //Method to call upon level ending
    private void EndLevel()
    {
        lastBoardEvent = EventState.EndLevel;
        EndLevelEvent.Invoke();
        TransitionToMap();
    }

    //Change scene to world map scene
    private void TransitionToMap()
    {
        SceneManager.LoadScene(mapSceneName);
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
    private IEnumerator EndTurnCounter()
    {
        if (new List<int>(actionsLeftDict.Values).TrueForAll(x => x == 0))
        {
            yield return new WaitForSeconds(EmptyExecutionTime);
        }
        while (!new List<int>(actionsLeftDict.Values).TrueForAll(x => x == 0))
        {
            yield return new WaitForSeconds(TimePerAction);
        }

        if (lastBoardEvent == EventState.PlayerExecute)
        {
            // Go to arthropod execute
            lastBoardEvent = EventState.PostPlayerExecute;
            PostPlayerExecuteEvent.Invoke();

            startExecuteTime = null;

            lastBoardEvent = EventState.StartArthropodTurn;
            StartArthropodTurnEvent.Invoke();

            lastBoardEvent = EventState.EndArthropodTurn;
            EndArthropodTurnEvent.Invoke();

            startExecuteTime = Time.time;

            lastBoardEvent = EventState.PreArthropodExecute;
            PreArthropodExecuteEvent.Invoke();

            lastBoardEvent = EventState.ArthropodExecute;
            ArthropodExecuteEvent.Invoke();

            StartCoroutine(EndTurnCounter());
        }
        else if(lastBoardEvent == EventState.ArthropodExecute)
        {
            lastBoardEvent = EventState.PostArthropodExecute;
            PostPlayerExecuteEvent.Invoke();

            lastBoardEvent = EventState.StartPlayerTurn;
            StartPlayerTurnEvent.Invoke();

            startExecuteTime = null;
        }
        
    }

    /// <summary>
    /// Receiver to OnStartTurn message. Resets max actions for the next turn.
    /// </summary>
    private void OnStartPlayerTurn()
    {
        PushUndoStack();
        actionsLeftDict.Clear();
    }

    private void OnPostPlayerExecute()
    {
        actionsLeftDict.Clear();
    }

    private void OnDestroy()
    {
        StartPlayerTurnEvent.RemoveAllListeners();
        EndPlayerTurnEvent.RemoveAllListeners();
        PostPlayerEndTurnEvent.RemoveAllListeners();

        PrePlayerExecuteEvent.RemoveAllListeners();
        PlayerExecuteEvent.RemoveAllListeners();
        PostPlayerExecuteEvent.RemoveAllListeners();

        StartArthropodTurnEvent.RemoveAllListeners();
        EndArthropodTurnEvent.RemoveAllListeners();

        PreArthropodExecuteEvent.RemoveAllListeners();
        ArthropodExecuteEvent.RemoveAllListeners();
        PostArthropodExecuteEvent.RemoveAllListeners();

        EndLevelEvent.RemoveAllListeners();

        ReadyEvent.RemoveAllListeners();
        BugsCaughtChangeEvent.RemoveAllListeners();
    }
}
