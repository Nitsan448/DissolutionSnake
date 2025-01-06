using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour, IDataPersistence
{
    [SerializeField] private TextMeshPro _text;
    private float _timeSinceGameStarted = 0;

    private void Start()
    {
        DataPersistenceManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        DataPersistenceManager.Instance.Unregister(this);
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != EGameState.Running) return;

        _timeSinceGameStarted += Time.deltaTime;
        SetText();
    }

    private void SetText()
    {
        _text.text = Mathf.RoundToInt(_timeSinceGameStarted).ToString();
    }

    public void SaveData(GameData dataToSave)
    {
        dataToSave.TimeSinceGameStarted = _timeSinceGameStarted;
    }

    public void LoadData(GameData loadedData)
    {
        _timeSinceGameStarted = loadedData.TimeSinceGameStarted;
        SetText();
    }
}