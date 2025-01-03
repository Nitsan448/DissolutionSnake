using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public SnakeBuilder SnakeBuilder { get; private set; }

    [SerializeField] private EDirection _startingMovementDirection = EDirection.Up;
    [SerializeField] private float _timeBetweenMovements;
    [SerializeField] private LayerMask _obstaclesLayerMask;
    [SerializeField] private SnakeNode _snakeNodePrefab;
    [SerializeField] private int _snakeStartingSize;

    private GameGrid _gameGrid;
    private GameManager _gameManager;
    private float _lastMovementTime;
    private PlayerInputHandler _playerInputHandler;

    public void Init(GameGrid gameGrid, GameManager gameManager)
    {
        _gameManager = gameManager;
        _gameGrid = gameGrid;
        _playerInputHandler = new PlayerInputHandler(_startingMovementDirection);
        SnakeBuilder = new SnakeBuilder(_gameGrid, _snakeNodePrefab, transform, _snakeStartingSize);
    }

    private void Start()
    {
        transform.position = _gameGrid.GetClosestTile(transform.position);
        SnakeBuilder.CreateSnake();
    }

    private void Update()
    {
        _playerInputHandler.HandleInput();
    }

    private void FixedUpdate()
    {
        bool isTimeForNextMovement = _lastMovementTime + _timeBetweenMovements < Time.time;
        if (isTimeForNextMovement || _playerInputHandler.DirectionChanged)
        {
            HandleCollisionsInNextTile();
            _lastMovementTime = Time.time;
            MoveToNextTile();
            _playerInputHandler.DirectionChanged = false;
        }
    }

    private void MoveToNextTile()
    {
        SnakeBuilder.Snake.First.Value.MakeBody();
        Vector2 newFrontPosition =
            _gameGrid.GetNextTileInDirection(SnakeBuilder.HeadPosition, _playerInputHandler.MovementDirection);
        SnakeBuilder.AddFront(newFrontPosition);
        SnakeBuilder.RemoveBack();
    }

    private void HandleCollisionsInNextTile()
    {
        Vector2 nextTilePosition = _gameGrid.GetNextTileInDirection(SnakeBuilder.HeadPosition, _playerInputHandler.MovementDirection);

        Vector2 rayCastDirection = nextTilePosition - SnakeBuilder.HeadPosition;
        RaycastHit2D hit = Physics2D.Raycast(SnakeBuilder.HeadPosition, rayCastDirection, _gameGrid.TileSize);

        if (hit)
        {
            HandleCollision(hit.collider.gameObject);
        }
    }

    private void HandleCollision(GameObject hit)
    {
        if (((1 << hit.layer) & _obstaclesLayerMask) != 0)
        {
            HitObstacle();
        }
        else if (hit.TryGetComponent(out Item item))
        {
            HitItem(hit.gameObject);
        }
    }

    public void HitObstacle()
    {
        _gameManager.ResetGame();
        _playerInputHandler.MovementDirection = _startingMovementDirection;
    }

    public void HitItem(GameObject item)
    {
        Destroy(item);
        _gameGrid.MarkTileAsUnOccupied(item.transform.position);
        SnakeBuilder.AddBack();
    }
}