public interface IDataPersistence
{
    //Every IDataPersistence implementer can access all parts of GameData, leading to potential misuse or unintended modifications.
    //It will be better if GameData split into smaller pieces, more focused classes that represent specific concerns:
    //then it would be IDataPersistence<T>

    public void SaveData(GameData dataToSave);

    public void LoadData(GameData loadedData);
}