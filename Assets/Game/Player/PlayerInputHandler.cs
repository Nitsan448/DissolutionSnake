using UnityEngine;

public class PlayerInputHandler
{
    public bool DirectionChanged = false;
    public EDirection MovementDirection;

    public PlayerInputHandler(EDirection movementDirection)
    {
        MovementDirection = movementDirection;
    }

    public void HandleInput()
    {
        //TODO: refactor
        EDirection previousDirection = MovementDirection;

        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && MovementDirection != EDirection.Left)
        {
            MovementDirection = EDirection.Right;
        }
        else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && MovementDirection != EDirection.Right)
        {
            MovementDirection = EDirection.Left;
        }
        else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && MovementDirection != EDirection.Down)
        {
            MovementDirection = EDirection.Up;
        }
        else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && MovementDirection != EDirection.Up)
        {
            MovementDirection = EDirection.Down;
        }

        DirectionChanged |= previousDirection != MovementDirection;
    }
}