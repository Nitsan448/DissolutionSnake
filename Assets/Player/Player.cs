using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Player : MonoBehaviour
{
    [ShowInInspector] [ReadOnly] private EDirection _direction = EDirection.Up;
    [SerializeField] private float _timeBetweenMovements;
    [SerializeField] private GameGrid _gameGrid;
    
    //Use object pooling for the snake sections

    private void Start()
    {
        transform.position = _gameGrid.GetClosestTile(transform.position);
        Move().Forget();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            _direction = _direction.RotateClockwise();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _direction = _direction.RotateCounterClockwise();
        }
    }

    private async UniTask Move()
    {
        //TODO: While game is running
        while (true)
        {
            GetComponent<Rigidbody2D>().MovePosition( _gameGrid.GetNextTileInDirection(transform.position,_direction));
            await UniTask.Delay(TimeSpan.FromSeconds(_timeBetweenMovements), delayTiming: PlayerLoopTiming.FixedUpdate);
        }
    }
}
