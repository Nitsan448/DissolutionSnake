using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : ASingleton<GameManager>
{
    [SerializeField] private Player _player;
    [SerializeField] private ScoreBoard _scoreBoard;
    [SerializeField] private UIManager _uiManager;
    [field: SerializeField] public GameGrid GameGrid { get; private set; }
    public EGameState GameState { get; private set; } = EGameState.Running;

    [SerializeField] private float _delayBetweenDeathAndRestart = 1;

    protected override void DoOnAwake()
    {
        _scoreBoard.Init(_player);
    }

    public async UniTask ResetGame()
    {
        GameState = EGameState.Finished;
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBetweenDeathAndRestart));
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

    private void PauseGame()
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