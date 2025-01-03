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
        //TODO: Refactor
        if (hit.TryGetComponent(out SnakeSegment snakeSegment))
        {
            LinkedListNode<SnakeSegment> current = _snakeBuilder.MiddleSegmentNode;
            while (current != null)
            {
                if (snakeSegment == current.Value)
                {
                    SplitSnake(current);
                    return;
                }

                current = current.Next;
            }
        }

        _gameManager.ResetGame();
    }

    private void SplitSnake(LinkedListNode<SnakeSegment> nodeToStartSplittingFrom)
    {
        // TODO: Set new middle

        LinkedList<SnakeSegment> splitSection = SplitSection(nodeToStartSplittingFrom);
        DissoulteSplitSection(splitSection).Forget();
    }

    private LinkedList<SnakeSegment> SplitSection(LinkedListNode<SnakeSegment> nodeToStartSplittingFrom)
    {
        // TODO: Refactor
        LinkedListNode<SnakeSegment> current = nodeToStartSplittingFrom;
        LinkedList<SnakeSegment> splitSection = new LinkedList<SnakeSegment>();

        while (current != null)
        {
            LinkedListNode<SnakeSegment> next = current.Next;
            splitSection.AddLast(current.Value);
            _snakeBuilder.RemoveSegmentWithoutDestroying(current);
            current = next;
        }

        return splitSection;
    }

    private async UniTask DissoulteSplitSection(LinkedList<SnakeSegment> splitSection)
    {
        foreach (SnakeSegment segment in splitSection)
        {
            _snakeBuilder.DestroySegment(segment);

            // TODO: Destroy each segment faster than the previous one
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
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