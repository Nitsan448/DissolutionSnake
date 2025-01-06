using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<Vector2> SnakeSegmentPositions = new();
    public List<Vector2> ItemPositions = new();
    public EDirection MovementDirection;
    public float TimeSinceGameStarted;
    public int CurrentScore;
}