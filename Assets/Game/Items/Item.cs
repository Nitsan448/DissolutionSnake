using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    [field: SerializeField] public int ItemScore { get; private set; } = 10;

    private ItemSpawner _itemSpawner;

    public void Init(ItemSpawner itemSpawner)
    {
        _itemSpawner = itemSpawner;
    }

    public void Remove()
    {
        //There is a circular dependency between Item and ItemSpawner. I believe Item shouldn't have knowledge of ItemSpawner.
        _itemSpawner.RemoveItem(this);
        GameManager.Instance.GameGrid.MarkTileAsUnOccupied(transform.position);
    }
}