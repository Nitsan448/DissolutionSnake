using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDataPersistence
{
    public Action<int> OnItemEaten;

    [SerializeField] private EDirection _startingMovementDirection = EDirection.Up;
    [SerializeField] private float _timeBetweenMovements;
    [SerializeField] private SnakeSegment _snakeSegmentPrefab;
    [SerializeField] private int _snakeStartingSize;
    [SerializeField] private float _snakeDissolutionStartingSpeed;
    [SerializeField] private AudioSource _deathAudioSource;
    [SerializeField] private AudioSource _eatItemAudioSource;
    [SerializeField] private AudioSource _eatTailAudioSource;

    private GameGrid _gameGrid;
    private float _lastMovementTime;
    private PlayerInputHandler _playerInputHandler;
    private SnakeBuilder _snakeBuilder;
    private SnakeSplitter _snakeSplitter;

    public void Init(GameGrid gameGrid)
    {
        _gameGrid = gameGrid;
        _playerInputHandler = new PlayerInputHandler(_startingMovementDirection);
        _snakeBuilder = new SnakeBuilder(_gameGrid, _snakeSegmentPrefab, transform, _snakeStartingSize);
        _snakeSplitter = new SnakeSplitter(_snakeBuilder, _snakeDissolutionStartingSpeed);
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
        if (GameManager.Instance.GameState != EGameState.Running) return;
        _playerInputHandler.HandleInput();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GameState != EGameState.Running) return;

        bool isReadyForNextMovement = _lastMovementTime + _timeBetweenMovements < Time.time;
        if (!isReadyForNextMovement && !_playerInputHandler.DirectionChanged) return;

        _lastMovementTime = Time.time;
        _playerInputHandler.DirectionChanged = false;

        Vector2 nextTilePosition = _gameGrid.GetNextTileInDirection(_snakeBuilder.HeadPosition, _playerInputHandler.MovementDirection);
        HandleCollisionsInNextTile(nextTilePosition);
        MoveToNextTile();
    }

    private void HandleCollisionsInNextTile(Vector2 nextTilePosition)
    {
        if (!_gameGrid.IsPositionOccupied(nextTilePosition)) return;

        GameObject hitObject = _gameGrid.GetGameObjectAtOccupiedTile(nextTilePosition);
        HandleCollision(hitObject);
    }

    private void HandleCollision(GameObject hitObject)
    {
        if (hitObject.TryGetComponent(out SnakeSegment snakeSegment))
        {
            HitSegment(snakeSegment);
        }
        else if (hitObject.TryGetComponent(out Walls walls))
        {
            Death();
        }
        else if (hitObject.TryGetComponent(out Item item))
        {
            HitItem(item);
        }
    }

    private void HitSegment(SnakeSegment snakeSegment)
    {
        if (snakeSegment.IsDetached) return;

        //Search for hit segment starting from middle node and ending at the tail
        LinkedListNode<SnakeSegment> current = _snakeBuilder.MiddleSegmentNode;
        while (current != null)
        {
            if (snakeSegment == current.Value)
            {
                _eatTailAudioSource.Play();
                _snakeSplitter.SplitSnake(current);
                return;
            }

            current = current.Next;
        }
    }

    private void Death()
    {
        _deathAudioSource.Play();
        GameManager.Instance.ResetGame().Forget();
    }

    private void HitItem(Item item)
    {
        OnItemEaten?.Invoke(item.ItemScore);
        Destroy(item.gameObject);
        _snakeBuilder.AddBack();
        _eatItemAudioSource.Play();
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