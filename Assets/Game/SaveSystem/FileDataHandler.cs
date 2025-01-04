using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string _saveFileDirectoryPath;
    private string _saveFileName;

    public FileDataHandler(string saveFileDirectoryPath, string saveFileName)
    {
        _saveFileDirectoryPath = saveFileDirectoryPath;
        _saveFileName = saveFileName;
    }

    public GameData TryLoadingData()
    {
        string fullPath = GetFullDataFilePath();
        if (!File.Exists(fullPath))
        {
            return null;
        }

        try
        {
            return LoadData(fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not load data from file " + fullPath + "\n" + e.Message);
            return null;
        }
    }

    private string GetFullDataFilePath()
    {
        return Path.Combine(_saveFileDirectoryPath, _saveFileName);
    }

    private GameData LoadData(string fullPath)
    {
        string dataToLoad = "";
        using (StreamReader reader = new StreamReader(fullPath))
        {
            dataToLoad = reader.ReadToEnd();
        }

        return JsonUtility.FromJson<GameData>(dataToLoad);
    }

    public void TrySavingData(GameData data)
    {
        string fullPath = GetFullDataFilePath();
        try
        {
            SaveData(data, fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not save data to file " + fullPath + "\n" + e.Message);
        }
    }

    private void SaveData(GameData data, string fullPath)
    {
        Directory.CreateDirectory(_saveFileDirectoryPath);

        string dataToStore = JsonUtility.ToJson(data, true);

        using (StreamWriter writer = new StreamWriter(fullPath))
        {
            writer.Write(dataToStore);
        }
    }
}