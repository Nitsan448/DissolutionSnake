using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private float _tileSize;
    
    //Do not save in a data structure, calculate each time.

    private Vector2 GetNearbyPosition(Vector2 positionInGrid, Vector2 direction)
    {
        direction.Normalize(); 
        Vector2 newPositionInGrid = positionInGrid + direction * _tileSize;
        return newPositionInGrid;
    }

    private void GetRandomTile()
    {
        Vector2 randomPositionInGrid = new(Random.Range(_collider.bounds.min.x, _collider.bounds.max.x),
            Random.Range(_collider.bounds.min.y, _collider.bounds.max.y));
    }

    private Vector2 GetClosestTile(Vector2 nearbyPosition)
    {
        return Vector2.zero;
    }
}
