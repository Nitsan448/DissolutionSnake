using System;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _headSprite;
    [SerializeField] private Sprite _segmentSprite;
    [SerializeField] private BoxCollider2D _collider;

    public void MakeHead()
    {
        _spriteRenderer.sprite = _headSprite;
    }

    public void MakeBody()
    {
        _spriteRenderer.sprite = _segmentSprite;
    }

    public void MakeMiddleNode()
    {
        _spriteRenderer.color = new Color(1, 1, 1, 0.7f);
    }

    public void MakeNormalNode()
    {
        _spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void DisableCollider()
    {
        _collider.enabled = false;
    }
}