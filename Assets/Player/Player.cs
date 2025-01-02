using System;
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

    //TODO: Use object pooling for the snake sections

    private void Awake()
    {
        _playerInputHandler = new PlayerInputHandler(_startingMovementDirection);
    }

    private void Start()
    {
        transform.position = _gameGrid.GetClosestTile(transform.position);
        _moveCts = new CancellationTokenSource();
        Move().Forget();
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
    }
}