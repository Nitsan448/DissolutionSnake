using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : ASingleton<GameManager>
{
    [SerializeField] private GameGrid _gameGrid;
    [SerializeField] private Player _player;
    [SerializeField] private ItemSpawner _itemSpawner;
    [SerializeField] private ScoreBoard _scoreBoard;
    [SerializeField] private TilePositionsGetter _tilePositionsGetter;
    public EGameState GameState { get; private set; } = EGameState.Running;

    protected override void DoOnAwake()
    {
        _player.Init(_gameGrid, this);
        _itemSpawner.Init(_gameGrid, this);
        _scoreBoard.Init(_player);
        _tilePositionsGetter.Init(_gameGrid);
    }

    public void ResetGame()
    {
        GameState = EGameState.Finished;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [Button]
    public void PauseGame()
    {
        GameState = EGameState.Paused;
    }

    [Button]
    public void ResumeGame()
    {
        GameState = EGameState.Running;
    }
}