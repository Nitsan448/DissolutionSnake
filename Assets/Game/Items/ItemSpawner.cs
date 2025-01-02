using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private float _timeBetweenSpawns;
    [SerializeField] private GameGrid _gameGrid;
    [SerializeField] private Item _itemPrefab;

    //Use object pooling for items?

    private void Start()
    {
        SpawnItems().Forget();
    }

    private async UniTask SpawnItems()
    {
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_timeBetweenSpawns));
            SpawnItem();
        }
    }

    private void SpawnItem()
    {
        Instantiate(_itemPrefab);
        _itemPrefab.transform.position = _gameGrid.GetRandomTile();
    }
}