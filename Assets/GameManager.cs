using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameGrid _gameGrid;
    [SerializeField] private Player _player;
    [SerializeField] private ItemSpawner _itemSpawner;

    private void Awake()
    {
        _player.Init(_gameGrid, this);
        _itemSpawner.Init(_gameGrid);
    }

    public void ResetGame()
    {
        //Resetting manually instead of reloading scene to prevent delay due to loading time
        _gameGrid.ResetGrid();
        _player.SnakeBuilder.DestroySnake();
        _player.SnakeBuilder.CreateSnake();
    }
}