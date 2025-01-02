using UnityEngine;

public class PlayerInputHandler
{
    public bool AcceptMovementInput = true;
    public EDirection MovementDirection;

    public PlayerInputHandler(EDirection movementDirection)
    {
        MovementDirection = movementDirection;
    }

    public void HandleInput()
    {
        //TODO: Move as soon as input is received, don't use accept movement input boolean

        if (!AcceptMovementInput) return;

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

        if (MovementDirection != previousDirection) AcceptMovementInput = false;
    }
}