using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameGrid _gameGrid;
    [SerializeField] private Player _player;
    [SerializeField] private ItemSpawner _itemSpawner;
    public EGameState GameState { get; private set; }

    private void Awake()
    {
        _player.Init(_gameGrid, this);
        _itemSpawner.Init(_gameGrid);
        GameState = EGameState.Running;
    }

    public void ResetGame()
    {
        GameState = EGameState.Finished;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}