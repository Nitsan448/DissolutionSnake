using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class SnakeBuilder : IDataPersistence, IDisposable
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
        DataPersistenceManager.Instance.Register(this);
    }

    public void Dispose()
    {
        DataPersistenceManager.Instance.Unregister(this);
    }

    public void CreateNewSnake()
    {
        Snake = new LinkedList<SnakeSegment>();
        AddFront(_playerTransform.position);
        MiddleSegmentNode = Snake.First;

        for (int i = 1; i < _snakeStartingSize; i++)
        {
            AddBack();
        }

        SetNewMiddleNode();
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
        SetNewMiddleNode();
        return;
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

    public void DetachSegment(LinkedListNode<SnakeSegment> segmentNode)
    {
        segmentNode.Value.DisableCollider();
        segmentNode.Value.MakeMiddleNode();
        Snake.Remove(segmentNode);
    }

    public void DestroySegment(SnakeSegment segment)
    {
        _gameGrid.MarkTileAsUnOccupied(segment.transform.position);
        Object.Destroy(segment.gameObject);
    }

    public void RemoveBack()
    {
        SnakeSegment last = Snake.Last.Value;
        Snake.RemoveLast();
        DestroySegment(last);
        UpdateMiddleNode(false);
    }

    public void SetNewMiddleNode()
    {
        MiddleSegmentNode?.Value.MakeNormalNode();

        LinkedListNode<SnakeSegment> current = Snake.First;
        for (int i = 0; i < Snake.Count / 2; i++)
        {
            current = current.Next;
        }

        MiddleSegmentNode = current;
        MiddleSegmentNode?.Value.MakeMiddleNode();
    }

    public void SaveData(GameData dataToSave)
    {
        LinkedListNode<SnakeSegment> current = Snake.First;
        while (current != null)
        {
            dataToSave.SnakeSegmentPositions.Add(current.Value.transform.position);
            current = current.Next;
        }
    }

    public void LoadData(GameData loadedData)
    {
        DestroySnake();
        CreateSnakeFromData(loadedData.SnakeSegmentPositions);
    }

    private void DestroySnake()
    {
        while (Snake.Count > 0)
        {
            RemoveBack();
        }
    }

    private void CreateSnakeFromData(List<Vector2> snakeData)
    {
        Snake.Clear();
        foreach (Vector2 snakeSegmentPosition in snakeData)
        {
            Snake.AddLast(CreateSnakeSegment(snakeSegmentPosition));
        }

        Snake.First.Value.MakeHead();
        SetNewMiddleNode();
    }
}