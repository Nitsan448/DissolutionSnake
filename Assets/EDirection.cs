using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDirection
{
    Right,
    Down,
    Left,
    Up,
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

    public static EDirection GetNextDirection(this EDirection direction)
    {
        if (direction == EDirection.Up)
        {
            return EDirection.Right;
        }

        return direction + 1;
    }
}