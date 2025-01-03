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

    public static EDirection GetDirectionFromVector(Vector2 vector)
    {
        vector = vector.normalized;

        if (vector == Vector2.up)
            return EDirection.Up;
        else if (vector == Vector2.down)
            return EDirection.Down;
        else if (vector == Vector2.left)
            return EDirection.Left;
        else if (vector == Vector2.right)
            return EDirection.Right;

        Debug.LogWarning("Vector is not in correct format");
        return EDirection.Up;
    }
}