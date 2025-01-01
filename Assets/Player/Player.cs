using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Player : MonoBehaviour
{
    [ShowInInspector] [ReadOnly] private EDirection _direction = EDirection.Up;
    [SerializeField] private float _timeBetweenMovements;
    [SerializeField] private GameGrid _gameGrid;
    private bool _acceptMovementInput = true;
    private CancellationTokenSource _moveCts;

    //Use object pooling for the snake sections

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
        if (_acceptMovementInput)
        {
            HandleMovementInput();
        }
    }

    private void HandleMovementInput()
    {
        //TODO: buffer input
        EDirection previousDirection = _direction;
        if (Input.GetKeyDown(KeyCode.D) && _direction != EDirection.Left)
        {
            _direction = EDirection.Right;
        }
        else if (Input.GetKeyDown(KeyCode.A) && _direction != EDirection.Right)
        {
            _direction = EDirection.Left;
        }
        else if (Input.GetKeyDown(KeyCode.W) && _direction != EDirection.Down)
        {
            _direction = EDirection.Up;
        }
        else if (Input.GetKeyDown(KeyCode.S) && _direction != EDirection.Up)
        {
            _direction = EDirection.Down;
        }

        if (_direction != previousDirection) _acceptMovementInput = false;
    }

    private async UniTask Move()
    {
        //TODO: While game is running
        while (true)
        {
            GetComponent<Rigidbody2D>().MovePosition(_gameGrid.GetNextTileInDirection(transform.position, _direction));
            _acceptMovementInput = true;
            await UniTask.Delay(TimeSpan.FromSeconds(_timeBetweenMovements), delayTiming: PlayerLoopTiming.FixedUpdate,
                cancellationToken: _moveCts.Token);
        }
    }
}