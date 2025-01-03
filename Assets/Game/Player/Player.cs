using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private EDirection _startingMovementDirection = EDirection.Up;
    [SerializeField] private float _timeBetweenMovements;
    [SerializeField] private LayerMask _obstaclesLayerMask;
    [SerializeField] private SnakeSegment _snakeSegmentPrefab;
    [SerializeField] private int _snakeStartingSize;

    private GameGrid _gameGrid;
    private GameManager _gameManager;
    private float _lastMovementTime;
    private PlayerInputHandler _playerInputHandler;
    private SnakeBuilder _snakeBuilder;

    public void Init(GameGrid gameGrid, GameManager gameManager)
    {
        _gameManager = gameManager;
        _gameGrid = gameGrid;
        _playerInputHandler = new PlayerInputHandler(_startingMovementDirection);
        _snakeBuilder = new SnakeBuilder(_gameGrid, _snakeSegmentPrefab, transform, _snakeStartingSize);
    }

    private void Start()
    {
        transform.position = _gameGrid.GetClosestTile(transform.position);
        _snakeBuilder.CreateSnake();
    }

    private void Update()
    {
        _playerInputHandler.HandleInput();
    }

    private void FixedUpdate()
    {
        if (_gameManager.GameState != EGameState.Running) return;

        bool isReadyForNextMovement = _lastMovementTime + _timeBetweenMovements < Time.time;
        if (!isReadyForNextMovement && !_playerInputHandler.DirectionChanged) return;

        _lastMovementTime = Time.time;
        _playerInputHandler.DirectionChanged = false;
        HandleCollisionsInNextTile();
        MoveToNextTile();
    }

    private void HandleCollisionsInNextTile()
    {
        Vector2 nextTilePosition = _gameGrid.GetNextTileInDirection(_snakeBuilder.HeadPosition, _playerInputHandler.MovementDirection);

        Vector2 rayCastDirection = nextTilePosition - _snakeBuilder.HeadPosition;
        RaycastHit2D hit = Physics2D.Raycast(_snakeBuilder.HeadPosition, rayCastDirection, _gameGrid.TileSize);

        if (hit)
        {
            HandleCollision(hit.collider.gameObject);
        }
    }

    private void HandleCollision(GameObject hit)
    {
        if (((1 << hit.layer) & _obstaclesLayerMask) != 0)
        {
            HitObstacle(hit);
        }
        else if (hit.TryGetComponent(out Item item))
        {
            HitItem(item);
        }
    }

    private void HitObstacle(GameObject hit)
    {
        //TODO: add snake division
        if (hit.TryGetComponent(out SnakeSegment snakeSegment))
        {
            LinkedListNode<SnakeSegment> current = _snakeBuilder.MiddleSegmentNode;
            while (current != null)
            {
                if (snakeSegment == current.Value)
                {
                    DivideSnake(current);
                    return;
                }

                current = current.Next;
            }
        }

        _gameManager.ResetGame();
    }

    private void DivideSnake(LinkedListNode<SnakeSegment> nodeToStartDividingFrom)
    {
        //TODO: add snake dissolution
        LinkedListNode<SnakeSegment> current = nodeToStartDividingFrom;
        while (current != null)
        {
            LinkedListNode<SnakeSegment> next = current.Next;
            _snakeBuilder.RemoveSegment(current);
            current = next;
        }
    }

    public void HitItem(Item item)
    {
        _gameGrid.MarkTileAsUnOccupied(item.transform.position);
        item.DestroyItem();
        _snakeBuilder.AddBack();
    }

    private void MoveToNextTile()
    {
        _snakeBuilder.Snake.First.Value.MakeBody();
        Vector2 newFrontPosition =
            _gameGrid.GetNextTileInDirection(_snakeBuilder.HeadPosition, _playerInputHandler.MovementDirection);
        _snakeBuilder.AddFront(newFrontPosition);
        _snakeBuilder.RemoveBack();
    }
}