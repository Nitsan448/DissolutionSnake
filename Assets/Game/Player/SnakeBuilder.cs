using System.Collections.Generic;
using UnityEngine;

public class SnakeBuilder
{
    //TODO: Use object pooling for the snake sections

    public LinkedList<SnakeSegment> Snake { get; private set; }
    public Vector2 HeadPosition => Snake.First.Value.transform.position;

    private GameGrid _gameGrid;
    private SnakeSegment _snakeSegmentPrefab;
    private Transform _playerTransform;
    private int _snakeStartingSize;
    public LinkedListNode<SnakeSegment> MiddleSegmentNode;

    public SnakeBuilder(GameGrid gameGrid, SnakeSegment snakeSegmentPrefab, Transform playerTransform, int snakeStartingSize)
    {
        _gameGrid = gameGrid;
        _snakeSegmentPrefab = snakeSegmentPrefab;
        _playerTransform = playerTransform;
        _snakeStartingSize = snakeStartingSize;
    }

    public void CreateSnake()
    {
        Snake = new LinkedList<SnakeSegment>();
        AddFront(_playerTransform.position);
        MiddleSegmentNode = Snake.First;

        for (int i = 1; i < _snakeStartingSize; i++)
        {
            AddBack();
        }
    }

    public void AddFront(Vector2 frontPosition)
    {
        Snake.AddFirst(CreateSnakeSegment(frontPosition));
        Snake.First.Value.MakeHead();
        _gameGrid.MarkTileAsOccupied(frontPosition, Snake.First.Value.gameObject);

        UpdateMiddleNode(false);
    }

    private void UpdateMiddleNode(bool moveForward)
    {
        if (Snake.Count % 2 == 0) return;
        if (MiddleSegmentNode == null) return;

        MiddleSegmentNode.Value.MakeNormalNode();
        MiddleSegmentNode = moveForward ? MiddleSegmentNode.Next : MiddleSegmentNode.Previous;
        MiddleSegmentNode?.Value.MakeMiddleNode();
    }

    private SnakeSegment CreateSnakeSegment(Vector2 segmentPosition)
    {
        return Object.Instantiate(_snakeSegmentPrefab, segmentPosition, Quaternion.identity, parent: _playerTransform);
    }

    public void AddBack()
    {
        SnakeSegment lastSegment = Snake.Last.Value;
        LinkedListNode<SnakeSegment> secondToLastSegment = Snake.Last.Previous;

        EDirection direction = secondToLastSegment == null
            ? EDirection.Down
            : EDirectionExtensions.GetDirectionFromVector(lastSegment.transform.position - secondToLastSegment.Value.transform.position);

        Vector2 newSegmentPosition = _gameGrid.GetNextTileInDirection(lastSegment.transform.position, direction);

        SnakeSegment newSegment = CreateSnakeSegment(newSegmentPosition);
        Snake.AddLast(newSegment);

        UpdateMiddleNode(true);
    }

    public void RemoveSegment(LinkedListNode<SnakeSegment> segment)
    {
        _gameGrid.MarkTileAsUnOccupied(segment.Value.transform.position);
        //TODO: optimize this, don't use Remove.
        Snake.Remove(segment);
        Object.Destroy(segment.Value.gameObject);
    }

    public void RemoveBack()
    {
        SnakeSegment last = Snake.Last.Value;
        _gameGrid.MarkTileAsUnOccupied(last.transform.position);
        Snake.RemoveLast();
        Object.Destroy(last.gameObject);

        UpdateMiddleNode(false);
    }
}