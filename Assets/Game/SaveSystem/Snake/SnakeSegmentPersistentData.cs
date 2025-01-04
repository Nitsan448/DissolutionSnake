using UnityEngine;

[System.Serializable]
public class SnakeSegmentPersistentData
{
    public Vector2 Position;

    public SnakeSegmentPersistentData(Vector2 position)
    {
        Position = position;
    }
}