using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class SnakeController : IDataPersistence, IDisposable
{
    public LinkedList<SnakeSegment> Snake { get; private set; }
    public Vector2 HeadPosition => Snake.First.Value.transform.position;

    private SnakeSegment _snakeSegmentPrefab;
    private Transform _playerTransform;
    private int _snakeStartingSize;
    private EDirection _startingDirection;
    public LinkedListNode<SnakeSegment> MiddleSegmentNode;


    public SnakeController(SnakeSegment snakeSegmentPrefab, Transform playerTransform, int snakeStartingSize, EDirection startingDirection)
    {
        _snakeSegmentPrefab = snakeSegmentPrefab;
        _playerTransform = playerTransform;
        _snakeStartingSize = snakeStartingSize;
        _startingDirection = startingDirection;
        DataPersistenceManager.Instance.Register(this);
    }

    public void Dispose()
    {
        DataPersistenceManager.Instance.Unregister(this);
    }

    public void CreateSnake()
    {
        Snake = new LinkedList<SnakeSegment>();
        AddFrontSegment(_playerTransform.position);

        for (int i = 1; i < _snakeStartingSize; i++)
        {
            AddBackSegment();
        }

        SetNewMiddleNode();
    }

    public void AddFrontSegment(Vector2 frontPosition)
    {
        SnakeSegment snakeSegment = CreateSnakeSegment(frontPosition);
        Snake.AddFirst(snakeSegment);
        Snake.First.Value.MakeHead();

        UpdateMiddleNode(false);
    }

    private SnakeSegment CreateSnakeSegment(Vector2 segmentPosition)
    {
        SnakeSegment createdSnakeSegment =
            Object.Instantiate(_snakeSegmentPrefab, segmentPosition, Quaternion.identity, parent: _playerTransform);
        GameManager.Instance.GameGrid.MarkTileAsOccupied(segmentPosition, createdSnakeSegment.gameObject);
        return createdSnakeSegment;
    }

    private void UpdateMiddleNode(bool moveForward)
    {
        bool shouldMoveMiddleNode = moveForward ? Snake.Count % 2 == 0 : Snake.Count % 2 == 1;
        if (!shouldMoveMiddleNode) return;
        if (MiddleSegmentNode == null)
        {
            SetNewMiddleNode();
            return;
        }

        MiddleSegmentNode.Value.MakeNormalNode();
        MiddleSegmentNode = moveForward ? MiddleSegmentNode.Next : MiddleSegmentNode.Previous;
        MiddleSegmentNode?.Value.MakeMiddleNode();
    }

    public void AddBackSegment()
    {
        Vector2 newSegmentPosition = GetNewBackPosition();

        SnakeSegment newSegment = CreateSnakeSegment(newSegmentPosition);
        Snake.AddLast(newSegment);

        UpdateMiddleNode(true);
    }

    private Vector2 GetNewBackPosition()
    {
        SnakeSegment lastSegment = Snake.Last.Value;
        LinkedListNode<SnakeSegment> secondToLastSegment = Snake.Last.Previous;

        EDirection direction = secondToLastSegment == null
            ? _startingDirection.GetOppositeDirection()
            : EDirectionExtensions.GetDirectionFromVector(lastSegment.transform.position - secondToLastSegment.Value.transform.position);

        return GameManager.Instance.GameGrid.GetNextTileInDirection(lastSegment.transform.position, direction);
    }

    public void DetachSegment(LinkedListNode<SnakeSegment> segmentNode)
    {
        segmentNode.Value.MakeDetachedNode();
        Snake.Remove(segmentNode);
    }

    public void RemoveBackSegment()
    {
        SnakeSegment last = Snake.Last.Value;
        Snake.RemoveLast();
        GameManager.Instance.GameGrid.MarkTileAsUnOccupied(last.transform.position);
        Object.Destroy(last.gameObject);
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
        LoadSnake(loadedData.SnakeSegmentPositions);
    }

    private void DestroySnake()
    {
        while (Snake.Count > 0)
        {
            RemoveBackSegment();
        }
    }

    private void LoadSnake(List<Vector2> snakeData)
    {
        foreach (Vector2 snakeSegmentPosition in snakeData)
        {
            SnakeSegment createdSnakedSegment = CreateSnakeSegment(snakeSegmentPosition);
            Snake.AddLast(createdSnakedSegment);
            GameManager.Instance.GameGrid.MarkTileAsOccupied(snakeSegmentPosition, createdSnakedSegment.gameObject);
        }

        Snake.First.Value.MakeHead();
        SetNewMiddleNode();
    }
}