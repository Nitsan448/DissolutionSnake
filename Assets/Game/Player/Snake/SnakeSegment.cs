using System;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    [SerializeField] private SnakeSegmentData _snakeSegmentData;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private GameGrid _gameGrid;

    public bool IsDetached { get; private set; } = false;

    public void Init(GameGrid gameGrid)
    {
        _gameGrid = gameGrid;
    }

    private void OnDestroy()
    {
        _gameGrid.MarkTileAsUnOccupied(transform.position);
    }

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