using UnityEngine;

public class Item : MonoBehaviour
{
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