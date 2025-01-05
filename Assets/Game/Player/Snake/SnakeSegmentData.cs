using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Data/Snake Segment", fileName = "Snake Segment Data")]
public class SnakeSegmentData : ScriptableObject
{
    public Sprite HeadSprite;
    public Sprite BodySprite;
    public float MiddleNodeAlpha = 0.5f;
    public float DetachedSegmentAlpha = 0.8f;
}