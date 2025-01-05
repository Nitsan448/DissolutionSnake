using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<Vector2> SnakeSegmentPositions;
    public List<Vector2> ItemPositions;
    public List<Vector2> TilePositions;
    public EDirection MovementDirection;
    public float TimeSinceGameStarted;
    public int CurrentScore;

    public GameData()
    {
        SnakeSegmentPositions = new List<Vector2>();
        ItemPositions = new List<Vector2>();
        TilePositions = new List<Vector2>();
    }
}