using System;
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
        return direction switch
        {
            EDirection.Right => Vector2.right,
            EDirection.Down => Vector2.down,
            EDirection.Left => Vector2.left,
            EDirection.Up => Vector2.up,
            _ => Vector2.zero
        };
    }

    public static EDirection GetDirectionFromVector(Vector2 vector)
    {
        vector = vector.normalized;

        if (vector == Vector2.up)
            return EDirection.Up;
        if (vector == Vector2.down)
            return EDirection.Down;
        if (vector == Vector2.left)
            return EDirection.Left;
        if (vector == Vector2.right)
            return EDirection.Right;

        return EDirection.Up;
    }

    public static EDirection GetOppositeDirection(this EDirection direction)
    {
        return (EDirection)(((int)direction + 2) % 4);
    }
}