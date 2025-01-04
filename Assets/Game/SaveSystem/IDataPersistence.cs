public interface IDataPersistence
{
    public void SaveData(GameData dataToSave);

    public void LoadData(GameData loadedData);
}