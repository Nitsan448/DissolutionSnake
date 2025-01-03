using System.Collections.Generic;
using UnityEngine;

public class SnakeBuilder
{
    //TODO: Use object pooling for the snake sections

    public LinkedList<SnakeNode> Snake { get; private set; }
    public Vector2 HeadPosition => Snake.First.Value.transform.position;

    private GameGrid _gameGrid;
    private SnakeNode _snakeNodePrefab;
    private Transform _playerTransform;

    public SnakeBuilder(GameGrid gameGrid, SnakeNode snakeNodePrefab, Transform playerTransform)
    {
        _gameGrid = gameGrid;
        _snakeNodePrefab = snakeNodePrefab;
        _playerTransform = playerTransform;
    }

    public void CreateSnake(int numberOfSegments)
    {
        Snake = new LinkedList<SnakeNode>();
        AddFront(_playerTransform.position);

        for (int i = 1; i < numberOfSegments; i++)
        {
            AddBack();
        }
    }

    public void AddFront(Vector2 frontPosition)
    {
        Snake.AddFirst(CreateSnakeSegment(frontPosition));
        Snake.First.Value.MakeHead();
        _gameGrid.MarkTileAsOccupied(frontPosition, Snake.First.Value.gameObject);
    }

    private SnakeNode CreateSnakeSegment(Vector2 segmentPosition)
    {
        return Object.Instantiate(_snakeNodePrefab, segmentPosition, Quaternion.identity, parent: _playerTransform);
    }

    public void AddBack()
    {
        SnakeNode lastSegment = Snake.Last.Value;
        LinkedListNode<SnakeNode> secondToLastSegment = Snake.Last.Previous;

        EDirection direction = secondToLastSegment == null
            ? EDirection.Down
            : EDirectionExtensions.GetDirectionFromVector(lastSegment.transform.position - secondToLastSegment.Value.transform.position);

        Vector2 newSegmentPosition = _gameGrid.GetNextTileInDirection(lastSegment.transform.position, direction);

        SnakeNode newSegment = CreateSnakeSegment(newSegmentPosition);
        Snake.AddLast(newSegment);
    }

    public void RemoveBack()
    {
        SnakeNode last = Snake.Last.Value;
        _gameGrid.MarkTileAsUnOccupied(last.transform.position);
        Snake.RemoveLast();
        Object.Destroy(last.gameObject);
    }

    public void DestroySnake()
    {
        while (Snake.Count > 0)
        {
            RemoveBack();
        }
        //TODO: also remove items from scene
        //TODO: call this from game manager
    }
}