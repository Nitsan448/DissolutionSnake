using UnityEngine;

public class Item : MonoBehaviour
{
    [field: SerializeField] public int ItemScore { get; private set; } = 10;

    private ItemSpawner _itemSpawner;

    public void Init(ItemSpawner itemSpawner)
    {
        _itemSpawner = itemSpawner;
    }

    public void DestroyItem()
    {
        _itemSpawner.RemoveItem(this);
        Destroy(gameObject);
    }
}