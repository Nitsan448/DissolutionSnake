using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDataPersistence
{
    //TODO: rename
    public Action<int> ItemEaten;

    [SerializeField] private EDirection _startingMovementDirection = EDirection.Up;
    [SerializeField] private float _timeBetweenMovements;
    [SerializeField] private LayerMask _obstaclesLayerMask;
    [SerializeField] private SnakeSegment _snakeSegmentPrefab;
    [SerializeField] private int _snakeStartingSize;
    [SerializeField] private float _snakeDissolutionStartingSpeed;

    private GameGrid _gameGrid;
    private GameManager _gameManager;
    private float _lastMovementTime;
    private PlayerInputHandler _playerInputHandler;
    private SnakeBuilder _snakeBuilder;
    private SnakeSplitter _snakeSplitter;
    private CollisionHandler _collisionHandler;

    public void Init(GameGrid gameGrid, GameManager gameManager)
    {
        _gameManager = gameManager;
        _gameGrid = gameGrid;
        _playerInputHandler = new PlayerInputHandler(_startingMovementDirection);
        _snakeBuilder = new SnakeBuilder(_gameGrid, _snakeSegmentPrefab, transform, _snakeStartingSize);
        _snakeSplitter = new SnakeSplitter(_snakeBuilder, _snakeDissolutionStartingSpeed);
        _collisionHandler = new CollisionHandler(this, _obstaclesLayerMask);
        DataPersistenceManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        DataPersistenceManager.Instance.Unregister(this);
    }

    private void Start()
    {
        transform.position = _gameGrid.GetClosestTile(transform.position);
        _snakeBuilder.CreateNewSnake();
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

        HandleCollisions();
        MoveToNextTile();
    }

    private void HandleCollisions()
    {
        Vector2 nextTilePosition = _gameGrid.GetNextTileInDirection(_snakeBuilder.HeadPosition, _playerInputHandler.MovementDirection);
        _collisionHandler.HandleCollisionsInNextTile(nextTilePosition, _snakeBuilder.HeadPosition, _gameGrid.TileSize);
    }


    public void HitObstacle(GameObject hit)
    {
        bool hitSnakeSegment = hit.TryGetComponent(out SnakeSegment snakeSegment);
        if (hitSnakeSegment)
        {
            //Search for hit segment starting from middle node and ending at the tail
            LinkedListNode<SnakeSegment> current = _snakeBuilder.MiddleSegmentNode;
            while (current != null)
            {
                if (snakeSegment == current.Value)
                {
                    _snakeSplitter.SplitSnake(current);
                    return;
                }

                current = current.Next;
            }
        }

        _gameManager.ResetGame();
    }

    public void HitItem(Item item)
    {
        ItemEaten?.Invoke(item.ItemScore);
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

    public void SaveData(GameData dataToSave)
    {
        dataToSave.MovementDirection = _playerInputHandler.MovementDirection;
    }

    public void LoadData(GameData loadedData)
    {
        _playerInputHandler.MovementDirection = loadedData.MovementDirection;
    }
}