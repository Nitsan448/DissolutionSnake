using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 _direction = Vector2.right;
    [SerializeField] private float _speed;
    [SerializeField] private GameGrid _gameGrid;
    
    //Use object pooling for the snake sections

    private void Start()
    {
        transform.position = _gameGrid.GetClosestTile(transform.position);
    }

    private void Update()
    {
        //Use axes and move to input handler
        if (Input.GetKeyDown(KeyCode.W))
        {
            _direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            _direction = Vector2.right;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _direction = Vector2.left;
        }
    }

    private void HandleCollisions()
    {
        
    }
    
}
