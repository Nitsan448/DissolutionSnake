using System;
using UnityEngine;

public class Item : AGridTile
{
    [field: SerializeField] public int ItemScore { get; private set; } = 10;

    private ItemSpawner _itemSpawner;

    public void Init(ItemSpawner itemSpawner)
    {
        _itemSpawner = itemSpawner;
    }

    private void OnDestroy()
    {
        _itemSpawner.RemoveItem(this);
    }
}