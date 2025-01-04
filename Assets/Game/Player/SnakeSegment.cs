using System;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _headSprite;
    [SerializeField] private Sprite _segmentSprite;
    [SerializeField] private BoxCollider2D _collider;

    private int _startingSortOrder;

    private void Start()
    {
        _startingSortOrder = _spriteRenderer.sortingOrder;
    }

    public void MakeHead()
    {
        _spriteRenderer.sprite = _headSprite;
        _spriteRenderer.sortingOrder = _startingSortOrder + 10;
    }

    public void MakeBody()
    {
        _spriteRenderer.sprite = _segmentSprite;
        _spriteRenderer.sortingOrder = _startingSortOrder;
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