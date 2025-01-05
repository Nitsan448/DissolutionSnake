using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class DataPersistenceManager : ASingleton<DataPersistenceManager>
{
    [SerializeField] private string _saveFileName;
    private FileDataHandler _fileDataHandler;
    private readonly List<IDataPersistence> _dataPersistenceObjects = new();
    private GameData _gameData;

    private void Start()
    {
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, _saveFileName);
    }

    public void Register(IDataPersistence dataPersistenceObject)
    {
        _dataPersistenceObjects.Add(dataPersistenceObject);
    }

    public void Unregister(IDataPersistence dataPersistenceObject)
    {
        _dataPersistenceObjects.Remove(dataPersistenceObject);
    }

    public void SaveGame()
    {
        _gameData = new GameData();

        foreach (IDataPersistence dataPersistenceObject in _dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(_gameData);
        }

        _fileDataHandler.TrySavingData(_gameData);
    }

    public void LoadGame()
    {
        _gameData = _fileDataHandler.TryLoadingData();
        if (_gameData == null)
        {
            _gameData = new GameData();
            return;
        }

        foreach (IDataPersistence dataPersistenceObject in _dataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(_gameData);
        }
    }
}