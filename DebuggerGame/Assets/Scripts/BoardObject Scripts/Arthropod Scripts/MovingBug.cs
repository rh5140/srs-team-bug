using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBug : Arthropod
{
    public bool myTurn;

    protected override void Start()
    {
        base.Start();
        myTurn = true;
    }
    protected override void OnStartTurn()
    {
        base.OnStartTurn();
        Player player = Board.instance.GetBoardObjectOfType<Player>();
        Vector2Int expectedMove = Vector2Int.zero;

        if(myTurn) {
            // player is directly above/below bug
            if(player.coordinate.x == this.coordinate.x) {
                if(player.coordinate.y < this.coordinate.y)
                    expectedMove = Vector2Int.up;
                if(player.coordinate.y > this.coordinate.y)
                    expectedMove = Vector2Int.down;
            }
            // player is to left of bug
            else if(player.coordinate.x < this.coordinate.x) {
                if(this.coordinate.x - player.coordinate.x <= this.coordinate.y - player.coordinate.y)
                    expectedMove = Vector2Int.up;
                else if(this.coordinate.x - player.coordinate.x < -(this.coordinate.y - player.coordinate.y))
                    expectedMove = Vector2Int.down;
                else
                    expectedMove = Vector2Int.right;
            }
            // player is to right of bug
            else {
                if(player.coordinate.x - this.coordinate.x < this.coordinate.y - player.coordinate.y)
                    expectedMove = Vector2Int.up;
                else if(player.coordinate.x - this.coordinate.x <= -(this.coordinate.y - player.coordinate.y))
                    expectedMove = Vector2Int.down;
                else
                    expectedMove = Vector2Int.left;
            }

            // check expected move and if it would move into a wall, move counterclockwise
            Vector2Int actualMove = expectedMove;
            if(!Board.instance.CanEnterCoordinate(this, this.coordinate + expectedMove)) {
                if(expectedMove == Vector2Int.left) {
                    if(player.coordinate.y >= this.coordinate.y) {
                        if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.down))
                            actualMove = Vector2Int.down;
                        else if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.right))
                            actualMove = Vector2Int.right;
                        else
                            actualMove = Vector2Int.up;
                    }
                    if(player.coordinate.y < this.coordinate.y) {
                        if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.up))
                            actualMove = Vector2Int.up;
                        else if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.right))
                            actualMove = Vector2Int.right;
                        else
                            actualMove = Vector2Int.down;
                    }
                }
                else if(expectedMove == Vector2Int.down) {
                    if(player.coordinate.x <= this.coordinate.x) {
                        if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.right))
                            actualMove = Vector2Int.right;
                        else if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.up))
                            actualMove = Vector2Int.up;
                        else
                            actualMove = Vector2Int.left;
                    }
                    if(player.coordinate.x > this.coordinate.x) {
                        if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.left))
                            actualMove = Vector2Int.left;
                        else if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.up))
                            actualMove = Vector2Int.up;
                        else
                            actualMove = Vector2Int.right;
                    }
                }
                else if(expectedMove == Vector2Int.right) {
                    if(player.coordinate.y <= this.coordinate.y) {
                        if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.up))
                            actualMove = Vector2Int.up;
                        else if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.left))
                            actualMove = Vector2Int.left;
                        else
                            actualMove = Vector2Int.down;
                    }
                    if(player.coordinate.y > this.coordinate.y) {
                        if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.down))
                            actualMove = Vector2Int.down;
                        else if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.left))
                            actualMove = Vector2Int.left;
                        else
                            actualMove = Vector2Int.up;
                    }
                }
                else if(expectedMove == Vector2Int.up) {
                    if(player.coordinate.x >= this.coordinate.x) {
                        if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.left))
                            actualMove = Vector2Int.left;
                        else if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.down))
                            actualMove = Vector2Int.down;
                        else
                            actualMove = Vector2Int.right;
                    }
                    if(player.coordinate.x < this.coordinate.x) {
                        if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.right))
                            actualMove = Vector2Int.right;
                        else if(Board.instance.CanEnterCoordinate(this, this.coordinate + Vector2Int.down))
                            actualMove = Vector2Int.down;
                        else
                            actualMove = Vector2Int.left;
                    }
                }
            }

            actions.Enqueue(new MovementAction(this, actualMove));

            myTurn = false;
        }
        else myTurn = true;
    }
}
