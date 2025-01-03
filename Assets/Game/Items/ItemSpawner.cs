using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private float _timeBetweenSpawns;
    [SerializeField] private Item _itemPrefab;

    private GameGrid _gameGrid;
    private CancellationTokenSource _spawnItemsCts;

    //Use object pooling for items?

    public void Init(GameGrid gameGrid)
    {
        _gameGrid = gameGrid;
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
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_timeBetweenSpawns), cancellationToken: _spawnItemsCts.Token);
                SpawnItem();
            }
        }
    }

    private void SpawnItem()
    {
        Vector2 itemPosition = _gameGrid.GetRandomUnoccupiedTile();
        Item item = Instantiate(_itemPrefab, itemPosition, Quaternion.identity, transform);
        _gameGrid.MarkTileAsOccupied(itemPosition, item.gameObject);
    }
}