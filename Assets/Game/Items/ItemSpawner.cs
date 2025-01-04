using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ItemSpawner : MonoBehaviour, IDataPersistence
{
    [SerializeField] private float _timeBetweenSpawns;
    [SerializeField] private int _maximumItems = 2;
    [SerializeField] private Item _itemPrefab;

    private GameGrid _gameGrid;
    private GameManager _gameManager;
    private CancellationTokenSource _spawnItemsCts;

    //Use object pooling for items?
    private List<Item> _items = new(2);

    public void Init(GameGrid gameGrid, GameManager gameManager)
    {
        _gameGrid = gameGrid;
        _gameManager = gameManager;
        DataPersistenceManager.Instance.Register(this);
    }

    private void Start()
    {
        SpawnItems().Forget();
    }

    private void OnDestroy()
    {
        _spawnItemsCts?.Cancel();
        DataPersistenceManager.Instance.Unregister(this);
    }

    private async UniTask SpawnItems()
    {
        using (_spawnItemsCts = new CancellationTokenSource())
        {
            while (!_spawnItemsCts.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_timeBetweenSpawns), cancellationToken: _spawnItemsCts.Token);
                //TODO: make max items a serialized filled
                if (_items.Count < _maximumItems && _gameManager.GameState == EGameState.Running) SpawnItem();
            }
        }
    }

    private void SpawnItem()
    {
        Vector2 itemPosition = _gameGrid.GetRandomUnoccupiedTile();
        SpawnItemAtPosition(itemPosition);
    }

    private void SpawnItemAtPosition(Vector2 position)
    {
        Item item = Instantiate(_itemPrefab, position, Quaternion.identity, transform);
        item.Init(this);
        _items.Add(item);
        _gameGrid.MarkTileAsOccupied(position, item.gameObject);
    }

    public void RemoveItem(Item item)
    {
        _items.Remove(item);
    }

    public void SaveData(GameData dataToSave)
    {
        foreach (Item item in _items)
        {
            dataToSave.ItemPositions.Add(item.transform.position);
        }
    }

    public void LoadData(GameData loadedData)
    {
        foreach (Item item in _items)
        {
            Destroy(item.gameObject);
        }

        _items.Clear();

        foreach (Vector2 itemPosition in loadedData.ItemPositions)
        {
            SpawnItemAtPosition(itemPosition);
        }
    }
}