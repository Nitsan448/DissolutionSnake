using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Player : MonoBehaviour
{
    [ShowInInspector] [ReadOnly] private EDirection _startingMovementDirection = EDirection.Up;
    [SerializeField] private float _timeBetweenMovements;
    [SerializeField] private GameGrid _gameGrid;
    private CancellationTokenSource _moveCts;
    private PlayerInputHandler _playerInputHandler;

    //Use object pooling for the snake sections

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
}