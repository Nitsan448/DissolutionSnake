using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<SnakeSegmentPersistentData> Snake;

    public GameData()
    {
        Snake = new List<SnakeSegmentPersistentData>();
    }
}