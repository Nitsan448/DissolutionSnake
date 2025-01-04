using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour, IDataPersistence
{
    [SerializeField] private TextMeshPro _text;
    private int _timeSinceGameStarted = 0;

    private void Start()
    {
        DataPersistenceManager.Instance.Register(this);
        CountTime().Forget();
    }

    private void OnDestroy()
    {
        DataPersistenceManager.Instance.Unregister(this);
    }

    private async UniTask CountTime()
    {
        while (true)
        {
            _text.text = _timeSinceGameStarted.ToString();
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            _timeSinceGameStarted += 1;
        }
    }

    public void SaveData(GameData dataToSave)
    {
        dataToSave.TimeSinceGameStarted = _timeSinceGameStarted;
    }

    public void LoadData(GameData loadedData)
    {
        _timeSinceGameStarted = loadedData.TimeSinceGameStarted;
        _text.text = _timeSinceGameStarted.ToString();
    }
}