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
        EDirection previousDirection = MovementDirection;
        if (Input.GetKeyDown(KeyCode.D) && MovementDirection != EDirection.Left)
        {
            MovementDirection = EDirection.Right;
        }
        else if (Input.GetKeyDown(KeyCode.A) && MovementDirection != EDirection.Right)
        {
            MovementDirection = EDirection.Left;
        }
        else if (Input.GetKeyDown(KeyCode.W) && MovementDirection != EDirection.Down)
        {
            MovementDirection = EDirection.Up;
        }
        else if (Input.GetKeyDown(KeyCode.S) && MovementDirection != EDirection.Up)
        {
            MovementDirection = EDirection.Down;
        }

        DirectionChanged |= previousDirection != MovementDirection;
    }
}