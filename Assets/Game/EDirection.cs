using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDirection
{
    Up,
    Right,
    Down,
    Left,
}

public static class EDirectionExtensions
{
    public static Vector2 GetDirectionVector(this EDirection direction)
    {
        switch (direction)
        {
            case EDirection.Right:
                return Vector2.right;
            case EDirection.Down:
                return Vector2.down;
            case EDirection.Left:
                return Vector2.left;
            case EDirection.Up:
                return Vector2.up;
        }

        return Vector2.zero;
    }

    public static EDirection RotateClockwise(this EDirection direction)
    {
        if (direction == EDirection.Left)
        {
            return EDirection.Up;
        }
        
        return direction + 1;
    }
    
    public static EDirection RotateCounterClockwise(this EDirection direction)
    {
        if (direction == EDirection.Up)
        {
            return EDirection.Left;
        }

        return direction - 1;
    }
}