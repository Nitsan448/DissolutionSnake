using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    [field: SerializeField] public int ItemScore { get; private set; } = 10;

    [SerializeField] private GameGrid _gameGrid;

    private ItemSpawner _itemSpawner;

    public void Init(ItemSpawner itemSpawner, GameGrid gameGrid)
    {
        _itemSpawner = itemSpawner;
        _gameGrid = gameGrid;
    }

    private void OnDestroy()
    {
        _itemSpawner.RemoveItem(this);
        _gameGrid.MarkTileAsUnOccupied(transform.position);
    }
}