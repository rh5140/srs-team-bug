using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : BoardObject
{
    public Arthropod heldArthropod;

    [System.Serializable]
    private enum FacingDirection
    {
        Left, Right, Up, Down
    }
    private FacingDirection facingDirection = FacingDirection.Right;

    public bool facingRight = true;
    public Animator animator;
    public AudioSource swallowSFX;
    private SpriteRenderer spriteRenderer;

    #region undo
    public override Dictionary<string, object> SaveState()
    {
        var dict = base.SaveState();
        dict.Add(
            nameof(Player),
            new Dictionary<string, object>
            {
                {nameof(facingDirection), facingDirection }
            }
        );
        return dict;
    }

    public override void LoadState(Dictionary<string, object> data)
    {
        base.LoadState(data);
        var playerData = (Dictionary<string, object>)data[nameof(Player)];
        facingDirection = (FacingDirection)playerData[nameof(facingDirection)];
        ApplyFacingDirection();
    }

    #endregion

    protected override void Start()
    {
        base.Start();
        if (SaveManager.instance != null) // Useful for testing individual levels withou having to start from the Save Menu
            SaveManager.instance.currentLevel = Board.instance.levelName;
        heldArthropod = null;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ApplyFacingDirection();
    }

    

    public void setArthropod(Arthropod heldArthropod)
    {
        this.heldArthropod = heldArthropod;
    }

    protected override void Update()
    {
        base.Update();

        if(board.gameState == Board.GameState.Paused)
        {
            return;
        }
        //Check for restart key (TEMP)
        float restart = Input.GetAxisRaw("Restart");
        if (!Mathf.Approximately(restart, 0f))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        //Instawin upon hitting correct key (TEMP)
        float win = Input.GetAxisRaw("Win");
        if (!Mathf.Approximately(win, 0f))
        {
            Board.instance.InstantWin();
        }

        if (board.lastBoardEvent == Board.EventState.StartPlayerTurn)
        {
            if (Input.GetButtonUp("Undo"))
            {
                board.Undo();
                return;
            }

            //release captured arthropod!
            float release = Input.GetAxisRaw("Release");
            if (!Mathf.Approximately(release, 0f) && heldArthropod != null)
            {
                heldArthropod.Release(this.gameObject);
            }

            float swallow = Input.GetAxisRaw("Swallow");
            if (!Mathf.Approximately(swallow, 0f) && heldArthropod != null)
            {
                if (swallowSFX != null)
                    swallowSFX.Play();
                heldArthropod.Swallow(this.gameObject);
            }

            // only move if during a turn

            Vector2 input = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );

            // check if input is nonzero
            // if it is, move in whatever direction in with component velocity 1
            Vector2Int direction = new Vector2Int(
                Mathf.Approximately(input.x, 0f) ? 0 : 1 * (int)Mathf.Sign(input.x),
                Mathf.Approximately(input.y, 0f) ? 0 : 1 * (int)Mathf.Sign(input.y)
            );

            // if x direction is nonzero, then set y direction to zero (no diagonal movement)
            direction.y = direction.x == 0 ? direction.y : 0;

            facingDirection =
                direction.x > 0
                ? FacingDirection.Right
                : direction.x < 0
                ? FacingDirection.Left
                : direction.y > 0
                ? FacingDirection.Up
                : direction.y < 0
                ? FacingDirection.Down
                : facingDirection;
            ApplyFacingDirection();

            if (!Mathf.Approximately(direction.magnitude, 0f))
            {
                // if we have input, add an action and then end turn
                actions.Enqueue(new MovementAction(this, direction));
                board.EndTurn();
            }

        }
    }

    protected override void OnStartPlayerTurn()
    {
        base.OnStartPlayerTurn();
        //Debug.Log("New Turn");
        /*
        Note: The bug overlap has to be checked for at the beginning of the turn since position has to update before we check if player is overlapping,
        however there is currently no implementation for actions to be executed at the beginning of turn 
        (they only currently execute at the execute phase), so this behavior is hardcoded for now in a slapdash manner.
        i.e bugs may play animations or have certain actions before being cause, which is not accounted for 
        in this implementation. Deletion of the bug during OnStartTurn might also mess with the logic of other
        board objects that rely on the existence of the bug(that is getting caught) to function.

        TODO: Action based implementation of the overlap checking.
        */
        //actions.Enqueue(DetectBugOverlap()); ?

        /* Physics based implementation for bug catching 
        Collider2D[] objectsOverlap = null;
        objectsOverlap = Physics2D.OverlapBoxAll((Vector2)this.transform.position, new Vector2(0.1f, 0.1f), 0f, int.MinValue, int.MaxValue);
        foreach (Collider2D newCol in objectsOverlap)
        {
            Debug.Log("Overlap");
            if (newCol.gameObject.GetComponent<Arthropod>() != null) //temporary identification for bug gameobjects (REPLACE THIS) 
            {
                newCol.gameObject.GetComponent<Arthropod>().Catch(); 
                board.BugCountUpdate();
                break;
            }
        }
        */

        CatchArthropods();
    }

    protected override void OnPostPlayerExecute()
    {
        base.OnPostPlayerExecute();

        CatchArthropods();
    }

    protected void CatchArthropods()
    {
        //Coordinate based implementation for bug catching
        foreach (Arthropod arthropod in board.GetBoardObjectsOfType<Arthropod>())
        {
            if (!arthropod.isCaught && arthropod.coordinate == this.coordinate && heldArthropod == null)
            {
                arthropod.Catch(this.gameObject);
                break;
            }
        }
    }

    //To be called when the level ends
    //Adds all of the unlockLevels in board to the unlockedLevels in SaveManager
    protected override void OnEndLevel()
    {
        base.OnEndLevel();
        SaveManager.instance.currentLevel = null;
        foreach (string levelName in Board.instance.unlockLevels)
        {
            SaveManager.instance.unlockedLevels.Add(levelName);
            SaveManager.instance.Save();
        }
        
    }
    
    private void ApplyFacingDirection()
    {
        spriteRenderer.flipX =
            facingDirection == FacingDirection.Left
            ? facingRight // if sprite is facing right, flip when facing left
            : facingDirection == FacingDirection.Right
            ? !facingRight
            : false;
        animator.SetBool("facingDown", facingDirection == FacingDirection.Down);
        animator.SetBool("facingUp", facingDirection == FacingDirection.Up);
        animator.SetBool("horizontal", facingDirection == FacingDirection.Left || facingDirection == FacingDirection.Right);
    }
}
