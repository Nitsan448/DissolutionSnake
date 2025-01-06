using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class SnakeBuilder : IDataPersistence, IDisposable
{
    //TODO: refactor

    public LinkedList<SnakeSegment> Snake { get; private set; }
    public Vector2 HeadPosition => Snake.First.Value.transform.position;

    private SnakeSegment _snakeSegmentPrefab;
    private Transform _playerTransform;
    private int _snakeStartingSize;
    public LinkedListNode<SnakeSegment> MiddleSegmentNode;


    public SnakeBuilder(SnakeSegment snakeSegmentPrefab, Transform playerTransform, int snakeStartingSize)
    {
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

        UpdateMiddleNode(false);
    }

    private void UpdateMiddleNode(bool moveForward)
    {
        //TODO: refactor
        bool moveMiddleNode = moveForward ? Snake.Count % 2 == 0 : Snake.Count % 2 == 1;
        if (!moveMiddleNode) return;
        if (MiddleSegmentNode == null)
        {
            SetNewMiddleNode();
            return;
        }

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
            ? EDirection.Down
            : EDirectionExtensions.GetDirectionFromVector(lastSegment.transform.position - secondToLastSegment.Value.transform.position);

        return GameManager.Instance.GameGrid.GetNextTileInDirection(lastSegment.transform.position, direction);
    }

    public void DetachSegment(LinkedListNode<SnakeSegment> segmentNode)
    {
        segmentNode.Value.MakeDetachedNode();
        Snake.Remove(segmentNode);
    }

    public void RemoveBack()
    {
        SnakeSegment last = Snake.Last.Value;
        Snake.RemoveLast();
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
            SnakeSegment createdSnakedSegment = CreateSnakeSegment(snakeSegmentPosition);
            Snake.AddLast(createdSnakedSegment);
        }

        Snake.First.Value.MakeHead();
        SetNewMiddleNode();
    }
}