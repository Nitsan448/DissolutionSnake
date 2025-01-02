using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private EDirection _startingMovementDirection = EDirection.Up;
    [SerializeField] private float _timeBetweenMovements;
    [SerializeField] private LayerMask _obstaclesLayerMask;
    [SerializeField] private GameGrid _gameGrid;
    private CancellationTokenSource _moveCts;
    private PlayerInputHandler _playerInputHandler;

    private LinkedList<SnakeNode> _snake;

    [SerializeField] private SnakeNode _snakeNodePrefab;

    //TODO: Use linked list for snake segments
    //TODO: Use object pooling for the snake sections

    private void Awake()
    {
        _playerInputHandler = new PlayerInputHandler(_startingMovementDirection);
    }

    private void Start()
    {
        CreateSnake(3);
        transform.position = _gameGrid.GetClosestTile(transform.position);
        _moveCts = new CancellationTokenSource();
        // Move().Forget();
    }

    private void CreateSnake(int numberOfSegments)
    {
        _snake = new LinkedList<SnakeNode>();
        _snake.AddFirst(Instantiate(_snakeNodePrefab, transform.position, Quaternion.identity));
        _snake.First.Value.MakeHead();

        for (int i = 1; i < numberOfSegments; i++)
        {
            _snake.AddLast(Instantiate(_snakeNodePrefab, transform.position + (new Vector3(0, -1, 0) * i), Quaternion.identity));
        }
    }

    private void AddFront()
    {
    }

    private void AddBack()
    {
    }

    private void OnDestroy()
    {
        _moveCts.Cancel();
    }

    private void Update()
    {
        _playerInputHandler.HandleInput();
    }


    private async UniTask Move()
    {
        //Each iteration 

        //TODO: While game is running
        while (true)
        {
            GetComponent<Rigidbody2D>()
                .MovePosition(_gameGrid.GetNextTileInDirection(transform.position, _playerInputHandler.MovementDirection));

            _playerInputHandler.AcceptMovementInput = true;
            await UniTask.Delay(TimeSpan.FromSeconds(_timeBetweenMovements), delayTiming: PlayerLoopTiming.FixedUpdate,
                cancellationToken: _moveCts.Token);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & _obstaclesLayerMask) != 0)
        {
            transform.position = new Vector3(0, -6, 0);
        }
        else if (other.gameObject.TryGetComponent(out Item item))
        {
            Destroy(other.gameObject);
        }
    }
}