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
    private float _lastMovementTime;
    private PlayerInputHandler _playerInputHandler;
    private SnakeController _snakeController;
    private SnakeSplitter _snakeSplitter;

    private void OnDestroy()
    {
        DataPersistenceManager.Instance.Unregister(this);
    }

    private void Start()
    {
        _playerInputHandler = new PlayerInputHandler(_startingMovementDirection);
        _snakeController = new SnakeController(_snakeSegmentPrefab, transform, _snakeStartingSize, _startingMovementDirection);
        _snakeSplitter = new SnakeSplitter(_snakeController, _snakeDissolutionStartingSpeed);
        DataPersistenceManager.Instance.Register(this);
        transform.position = GameManager.Instance.GameGrid.GetClosestTile(transform.position);
        _snakeController.CreateNewSnake();
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

        Vector2 nextTilePosition =
            GameManager.Instance.GameGrid.GetNextTileInDirection(_snakeController.HeadPosition, _playerInputHandler.MovementDirection);
        HandleCollisionsInNextTile(nextTilePosition);
        MoveToNextTile();
    }

    private void HandleCollisionsInNextTile(Vector2 nextTilePosition)
    {
        if (!GameManager.Instance.GameGrid.IsPositionOccupied(nextTilePosition)) return;
        GameObject hitObject = GameManager.Instance.GameGrid.GetGameObjectAtOccupiedTile(nextTilePosition);

        // Found some last minute bugs and had to do this null check :(
        // Items in the grid are sometimes marked as null after saving and loading the game.
        // This happens when the game is saved with items in the grid
        // and then loaded while those items are still there.
        if (hitObject == null) return;

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

        LinkedListNode<SnakeSegment> current = _snakeController.MiddleSegmentNode;
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

        Death();
    }

    private void Death()
    {
        _deathAudioSource.Play();
        GameManager.Instance.ResetGame().Forget();
    }

    private void HitItem(Item item)
    {
        OnItemEaten?.Invoke(item.ItemScore);
        item.Remove();
        Destroy(item.gameObject);
        _snakeController.AddBack();
        _eatItemAudioSource.Play();
    }

    private void MoveToNextTile()
    {
        _snakeController.Snake.First.Value.MakeBody();
        Vector2 newFrontPosition =
            GameManager.Instance.GameGrid.GetNextTileInDirection(_snakeController.HeadPosition, _playerInputHandler.MovementDirection);
        _snakeController.AddFront(newFrontPosition);
        _snakeController.RemoveBack();
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