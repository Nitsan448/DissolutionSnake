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

    private float _timeSinceLastItemSpawn;

    //Use object pooling for items?
    private List<Item> _items = new(2);

    private void Start()
    {
        DataPersistenceManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        DataPersistenceManager.Instance.Unregister(this);
    }


    private void Update()
    {
        if (GameManager.Instance.GameState != EGameState.Running) return;
        _timeSinceLastItemSpawn += Time.deltaTime;
        if (_items.Count >= _maximumItems) return;

        if (_timeSinceLastItemSpawn > _timeBetweenSpawns)
        {
            SpawnItem();
            _timeSinceLastItemSpawn = 0;
        }
    }

    private void SpawnItem()
    {
        Vector2 itemPosition = GameManager.Instance.GameGrid.GetRandomUnoccupiedTile();
        SpawnItemAtPosition(itemPosition);
    }

    private void SpawnItemAtPosition(Vector2 position)
    {
        Item item = Instantiate(_itemPrefab, position, Quaternion.identity, transform);
        item.Init(this);
        _items.Add(item);
        GameManager.Instance.GameGrid.MarkTileAsOccupied(position, item.gameObject);
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
        DestroyAllItems();

        foreach (Vector2 itemPosition in loadedData.ItemPositions)
        {
            SpawnItemAtPosition(itemPosition);
        }
    }

    private void DestroyAllItems()
    {
        foreach (Item item in _items)
        {
            Destroy(item.gameObject);
        }
    }
}