using System.Collections.Generic;
using UnityEngine;

public class SnakeNode : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _headSprite;
    [SerializeField] private Sprite _segmentSprite;
    [SerializeField] private BoxCollider2D _collider;

    public void MakeHead()
    {
        _spriteRenderer.sprite = _headSprite;
        _collider.enabled = false;
    }

    public void MakeBody()
    {
        _spriteRenderer.sprite = _segmentSprite;
        _collider.enabled = true;
    }
}