using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovingArthropodBehavior : ArthropodBehavior
{
    bool currentTurn = false;

    #region undo

    public override Dictionary<string, object> SaveState()
    {
        var dict = base.SaveState();
        dict.Add(
            nameof(MovingArthropodBehavior),
            new Dictionary<string, object>
            {
                {nameof(currentTurn), currentTurn},
            }
        );
        return dict;
    }

    public override void LoadState(Dictionary<string, object> data)
    {
        base.LoadState(data);
        var behaviorState = (Dictionary<string, object>) data[nameof(MovingArthropodBehavior)];
        currentTurn = (bool) behaviorState[nameof(currentTurn)];
    }

    #endregion

    public override void OnEndArthropodTurn()
    {
        base.OnEndArthropodTurn();
        if (arthropod.isCaught)
        {
            currentTurn = true;

            // OUTDATED WAY TO FOLLOW THE PLAYER WHEN CAUGHT
            // Follow the player if the bug is caught in the player's mouth
            // TODO - Clear the queue of movement actions if the bug is released. Otherwise the bug will try to move to the player one more time.
            //if (isCaught)
            //    actions.Enqueue(new MovementAction(this, playerDist));
        }
        else if (currentTurn)
        {
            Player player = Board.instance.GetBoardObjectOfType<Player>();

            bool[] canMove = { false, false, false, false }; // 0 = UP, 1 = LEFT, 2 = DOWN, 3 = RIGHT

            // relative position of the player with respect to the bug
            Vector2Int playerDist = new Vector2Int(player.coordinate.x - arthropod.coordinate.x, player.coordinate.y - arthropod.coordinate.y);

            // check for collidables in the four surrounding tiles
            if (Board.instance.CanEnterCoordinate(arthropod, arthropod.coordinate + Vector2Int.up)) { canMove[0] = true; }
            if (Board.instance.CanEnterCoordinate(arthropod, arthropod.coordinate + Vector2Int.left)) { canMove[1] = true; }
            if (Board.instance.CanEnterCoordinate(arthropod, arthropod.coordinate + Vector2Int.down)) { canMove[2] = true; }
            if (Board.instance.CanEnterCoordinate(arthropod, arthropod.coordinate + Vector2Int.right)) { canMove[3] = true; }

            // Debug.Log(playerDist);

            // check for the player in the four surrounding tiles
            if (playerDist.x == 0 && playerDist.y == 1) { canMove[0] = false; }
            if (playerDist.x == -1 && playerDist.y == 0) { canMove[1] = false; }
            if (playerDist.x == 0 && playerDist.y == -1) { canMove[2] = false; }
            if (playerDist.x == 1 && playerDist.y == 0) { canMove[3] = false; }

            Vector2Int actualMove = Vector2Int.zero;

            // player is directly above/below bug
            if (playerDist.x == 0)
            {
                if (playerDist.y < 0) // player is directly below the bug. Try moving up, then CW
                    actualMove = tryToMove(new int[] { 0, 1, 3, 2 }, canMove);
                else if (playerDist.y > 0) // player is directly above the bug. Try moving down, then CW
                    actualMove = tryToMove(new int[] { 2, 3, 1, 0 }, canMove);
            }

            // player is to left of bug
            else if (playerDist.x < 0)
            {
                if (-playerDist.x < -playerDist.y) // player is to the lower-left. Try moving up, then right, then CW
                    actualMove = tryToMove(new int[] { 0, 3, 1, 2 }, canMove);
                else if (-playerDist.x < playerDist.y) // player is to the upper-left. Try moving down, then right, then left
                    actualMove = tryToMove(new int[] { 2, 3, 1, 0 }, canMove);
                else if (playerDist.y > 0) // player is slightly above horizontal axis to the left
                    actualMove = tryToMove(new int[] { 3, 2, 0, 1 }, canMove);
                else // player is directly to the left or slightly below horizontal axis
                    actualMove = tryToMove(new int[] { 3, 0, 2, 1 }, canMove);
            }

            // player is to right of bug
            else if (playerDist.x > 0)
            {
                if (playerDist.x < -playerDist.y) // player is to the lower-right. Try moving up, then left, then right
                    actualMove = tryToMove(new int[] { 0, 1, 3, 2 }, canMove);
                else if (playerDist.x <= playerDist.y) // player is to the upper-right. Try moving down, then left, then right
                    actualMove = tryToMove(new int[] { 2, 1, 3, 0 }, canMove);
                else if (playerDist.y < 0) // player is slightly below horizontal axis to the right
                    actualMove = tryToMove(new int[] { 1, 0, 2, 3 }, canMove);
                else // player is directly to the right or slightly above horizontal axis
                    actualMove = tryToMove(new int[] { 1, 2, 0, 3 }, canMove);
            }

            arthropod.actions.Enqueue(new MovementAction(arthropod, actualMove));
            currentTurn = false;
        }
        else
        {
            currentTurn = true;
        }
    }

    private Vector2Int tryToMove(int[] dir, bool[] canMove)
    {
        int expectedMove = -1;
        if (canMove[dir[0]]) { expectedMove = dir[0]; }
        else if (canMove[dir[1]]) { expectedMove = dir[1]; }
        else if (canMove[dir[2]]) { expectedMove = dir[2]; }
        else if (canMove[dir[3]]) { expectedMove = dir[3]; }
        else { expectedMove = -1; }

        if (expectedMove == 0) { return Vector2Int.up; }
        if (expectedMove == 1) { return Vector2Int.left; }
        if (expectedMove == 2) { return Vector2Int.down; }
        if (expectedMove == 3) { return Vector2Int.right; }

        return Vector2Int.zero;
    }
}
