using System;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _headSprite;
    [SerializeField] private Sprite _segmentSprite;
    [SerializeField] private float _middleNodeAlpha = 0.5f;
    [SerializeField] private float _detachedSegmentAlpha = 0.8f;
    [SerializeField] private BoxCollider2D _collider;

    public void MakeHead()
    {
        _spriteRenderer.sprite = _headSprite;
    }

    public void MakeBody()
    {
        _spriteRenderer.sprite = _segmentSprite;
    }

    public void MakeDetachedNode()
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _detachedSegmentAlpha);
        _spriteRenderer.sortingOrder -= 10;
    }

    public void MakeMiddleNode()
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _middleNodeAlpha);
    }

    public void MakeNormalNode()
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1);
    }

    public void DisableCollider()
    {
        _collider.enabled = false;
    }
}