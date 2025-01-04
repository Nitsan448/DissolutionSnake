using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
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
    }

    private void Start()
    {
        SpawnItems().Forget();
    }

    private void OnDestroy()
    {
        _spawnItemsCts?.Cancel();
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
        Item item = Instantiate(_itemPrefab, itemPosition, Quaternion.identity, transform);
        item.Init(this);
        _items.Add(item);
        _gameGrid.MarkTileAsOccupied(itemPosition, item.gameObject);
    }

    public void RemoveItem(Item item)
    {
        _items.Remove(item);
    }
}