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
    [SerializeField] private TilemapGridMarker _tilemapGridMarker;
    [SerializeField] private UIManager _uiManager;
    public EGameState GameState { get; private set; } = EGameState.Running;

    protected override void DoOnAwake()
    {
        _player.Init(_gameGrid, this);
        _itemSpawner.Init(_gameGrid, this);
        _scoreBoard.Init(_player);
        _tilemapGridMarker.Init(_gameGrid);
    }

    public void ResetGame()
    {
        GameState = EGameState.Finished;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UpdatePausedState();
        }
    }

    private void UpdatePausedState()
    {
        if (GameState == EGameState.Running)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    
    public void PauseGame()
    {
        _uiManager.FadeInPausePanel();
        GameState = EGameState.Paused;
    }
    
    public void ResumeGame()
    {
        _uiManager.FadeOutPausePanel();
        GameState = EGameState.Running;
    }
}