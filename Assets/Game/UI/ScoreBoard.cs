using System;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour, IDataPersistence
{
    [SerializeField] private TextMeshPro _text;

    private Player _player;
    private int _currentScore;

    //TODO: refactor
    private bool _subscribedToItemEatenEvent = false;

    public void Init(Player player)
    {
        _player = player;
        SetScore(0);

        if (_subscribedToItemEatenEvent) return;
        _subscribedToItemEatenEvent = true;
        _player.OnItemEaten += UpdateScore;
        DataPersistenceManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        DataPersistenceManager.Instance.Unregister(this);
    }

    private void SetScore(int score)
    {
        _currentScore = score;
        _text.text = _currentScore.ToString();
    }

    private void OnEnable()
    {
        if (_player == null || _subscribedToItemEatenEvent) return;

        _subscribedToItemEatenEvent = true;
        _player.OnItemEaten += UpdateScore;
    }

    private void OnDisable()
    {
        _player.OnItemEaten -= UpdateScore;
        _subscribedToItemEatenEvent = false;
    }

    private void UpdateScore(int scoreChange)
    {
        SetScore(_currentScore + scoreChange);
    }

    public void SaveData(GameData dataToSave)
    {
        dataToSave.CurrentScore = _currentScore;
    }

    public void LoadData(GameData loadedData)
    {
        SetScore(loadedData.CurrentScore);
    }
}