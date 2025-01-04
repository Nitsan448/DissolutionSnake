using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<Vector2> SnakeSegmentPositions;
    public List<Vector2> ItemPositions;
    public EDirection MovementDirection;

    public GameData()
    {
        SnakeSegmentPositions = new List<Vector2>();
        ItemPositions = new List<Vector2>();
        MovementDirection = EDirection.Up;
    }
}