using System;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment : AGridTile
{
    [SerializeField] private SnakeSegmentData _snakeSegmentData;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public bool IsDetached { get; private set; } = false;

    public void MakeHead()
    {
        _spriteRenderer.sprite = _snakeSegmentData.HeadSprite;
    }

    public void MakeBody()
    {
        _spriteRenderer.sprite = _snakeSegmentData.BodySprite;
    }

    public void MakeDetachedNode()
    {
        _spriteRenderer.color = _spriteRenderer.color.GetWithNewAlpha(_snakeSegmentData.DetachedSegmentAlpha);
        _spriteRenderer.sortingOrder -= 10;
        IsDetached = true;
        GameManager.Instance.GameGrid.MarkTileAsUnOccupied(transform.position);
    }

    public void MakeMiddleNode()
    {
        _spriteRenderer.color = _spriteRenderer.color.GetWithNewAlpha(_snakeSegmentData.MiddleNodeAlpha);
    }

    public void MakeNormalNode()
    {
        _spriteRenderer.color = _spriteRenderer.color.GetWithNewAlpha(1);
    }
}