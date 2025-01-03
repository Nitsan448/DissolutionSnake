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
    private float _lastMovementTime;
    private Rigidbody2D _rigidbody;
    private PlayerInputHandler _playerInputHandler;

    public LinkedList<SnakeNode> Snake { get; private set; }


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
    }

    private void CreateSnake(int numberOfSegments)
    {
        Snake = new LinkedList<SnakeNode>();
        AddFront(transform.position);

        for (int i = 1; i < numberOfSegments; i++)
        {
            AddBack();
            // Snake.AddLast(CreateSnakeSegment(transform.position + (Vector3.down * i)));
        }
    }

    private void AddFront(Vector2 frontPosition)
    {
        Snake.AddFirst(CreateSnakeSegment(frontPosition));
        Snake.First.Value.MakeHead();
        _gameGrid.MarkTileAsOccupied(frontPosition, Snake.First.Value.gameObject);
    }

    private SnakeNode CreateSnakeSegment(Vector2 segmentPosition)
    {
        return Instantiate(_snakeNodePrefab, segmentPosition, Quaternion.identity, parent: transform);
    }

    private void Update()
    {
        _playerInputHandler.HandleInput();
    }

    private void FixedUpdate()
    {
        if (!(_lastMovementTime + _timeBetweenMovements < Time.time)) return;

        HandleCollisionsInNextTile();
        _lastMovementTime = Time.time;
        MoveToNextTile();
        _playerInputHandler.AcceptMovementInput = true;
    }

    private void MoveToNextTile()
    {
        Snake.First.Value.MakeBody();
        Vector2 newFrontPosition =
            _gameGrid.GetNextTileInDirection(Snake.First.Value.transform.position, _playerInputHandler.MovementDirection);
        AddFront(newFrontPosition);
        RemoveBack();
    }

    private void RemoveBack()
    {
        SnakeNode last = Snake.Last.Value;
        _gameGrid.MarkTileAsUnOccupied(last.transform.position);
        Snake.RemoveLast();
        Destroy(last.gameObject);
    }

    private void HandleCollisionsInNextTile()
    {
        Vector2 headPosition = Snake.First.Value.transform.position;
        Vector2 nextTilePosition = _gameGrid.GetNextTileInDirection(headPosition, _playerInputHandler.MovementDirection);

        RaycastHit2D hit = Physics2D.Raycast(headPosition, nextTilePosition - headPosition, _gameGrid.TileSize);

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
        DestroySnake();
        CreateSnake(_snakeStartingSize);
        _playerInputHandler.MovementDirection = _startingMovementDirection;
    }

    private void DestroySnake()
    {
        foreach (SnakeNode segment in Snake)
        {
            Destroy(segment.gameObject);
        }

        // Clear the linked list
        Snake.Clear();

        //TODO: also remove items from scene
        //TODO: move this to game manager
    }

    public void HitItem(GameObject item)
    {
        Destroy(item);
        _gameGrid.MarkTileAsUnOccupied(item.transform.position);
        AddBack();
    }

    private void AddBack()
    {
        SnakeNode lastSegment = Snake.Last.Value;
        LinkedListNode<SnakeNode> secondToLastSegment = Snake.Last.Previous;

        EDirection direction = secondToLastSegment == null
            ? EDirection.Down
            : EDirectionExtensions.GetDirectionFromVector(lastSegment.transform.position - secondToLastSegment.Value.transform.position);

        Vector2 newSegmentPosition = _gameGrid.GetNextTileInDirection(lastSegment.transform.position, direction);

        SnakeNode newSegment = CreateSnakeSegment(newSegmentPosition);
        Snake.AddLast(newSegment);
    }
}