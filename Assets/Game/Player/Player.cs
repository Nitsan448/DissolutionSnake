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
    [SerializeField] private GameGrid _gameGrid;
    [SerializeField] private SnakeNode _snakeNodePrefab;
    [SerializeField] private int _snakeStartingSize;
    private Rigidbody2D _rigidbody;
    private CancellationTokenSource _moveCts;
    private PlayerInputHandler _playerInputHandler;

    private LinkedList<SnakeNode> _snake;


    //TODO: Use linked list for snake segments
    //TODO: Use object pooling for the snake sections

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInputHandler = new PlayerInputHandler(_startingMovementDirection);
    }

    private void Start()
    {
        transform.position = _gameGrid.GetClosestTile(transform.position);
        CreateSnake(_snakeStartingSize);
        _moveCts = new CancellationTokenSource();
        Move().Forget();
    }

    private void CreateSnake(int numberOfSegments)
    {
        _snake = new LinkedList<SnakeNode>();
        _snake.AddFirst(CreateSnakeSegment(transform.position));
        _snake.First.Value.MakeHead();

        for (int i = 1; i < numberOfSegments; i++)
        {
            _snake.AddLast(CreateSnakeSegment(transform.position + (Vector3.down * i)));
        }
    }

    private SnakeNode CreateSnakeSegment(Vector2 segmentPosition)
    {
        return Instantiate(_snakeNodePrefab, segmentPosition, Quaternion.identity, parent: transform);
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
        //TODO: While game is running
        while (true)
        {
            //TODO: instead of simply moving, create a new head and remove the tail.
            //TODO: Custom collision detection is likely needed.

            Vector2 nextTilePosition = _gameGrid.GetNextTileInDirection(_snake.First.Value.transform.position,
                _playerInputHandler.MovementDirection);
            HandleCollisionsInNextTile(nextTilePosition);
            _snake.First.Value.MakeBody();
            _snake.AddFirst(CreateSnakeSegment(_gameGrid.GetNextTileInDirection(_snake.First.Value.transform.position,
                _playerInputHandler.MovementDirection)));
            _snake.First.Value.MakeHead();
            SnakeNode last = _snake.Last.Value;
            _snake.RemoveLast();
            Destroy(last.gameObject);

            _playerInputHandler.AcceptMovementInput = true;
            await UniTask.Delay(TimeSpan.FromSeconds(_timeBetweenMovements), delayTiming: PlayerLoopTiming.FixedUpdate,
                cancellationToken: _moveCts.Token);
        }
    }


    private void HandleCollisionsInNextTile(Vector2 nextTilePosition)
    {
        //Problem with overlap point
        Collider2D hit = Physics2D.OverlapBox(nextTilePosition, _gameGrid.TileSize * Vector2.one, 0);
        if (!hit) return;
        Debug.Log(hit.gameObject.name);
        if (((1 << hit.gameObject.layer) & _obstaclesLayerMask) != 0)
        {
            HitObstacle();
        }
        else if (hit.gameObject.TryGetComponent(out Item item))
        {
            HitItem(hit.gameObject);
        }
    }

    public void HitObstacle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void HitItem(GameObject item)
    {
        Destroy(item);
    }
}