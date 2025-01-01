using Sirenix.OdinInspector;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private float _tileSize;
    
    //Do not save in a data structure, calculate each time.

    private Vector2 GetNextTileInDirection(Vector2 positionInGrid, EDirection direction)
    {
        Vector2 newPositionInGrid = positionInGrid + direction.GetDirectionVector() * _tileSize;
        return newPositionInGrid;
    }

    private Vector2 GetRandomTile()
    {
        Vector2 randomPositionInGrid = new(Random.Range(_collider.bounds.min.x, _collider.bounds.max.x),
            Random.Range(_collider.bounds.min.y, _collider.bounds.max.y));
        return GetClosestTile(randomPositionInGrid);
    }

    [Button]
    public Vector2 GetClosestTile(Vector2 positionInGrid)
    {
        //TODO: make this work with a position outside of the grid
        Vector2 gridStartTile = _collider.bounds.min;
        
        float tileRowIndex = Mathf.Round((positionInGrid.x - gridStartTile.x) / _tileSize);
        float tileColumnIndex = Mathf.Round((positionInGrid.y - gridStartTile.y) / _tileSize);

        Vector2 closestTile = new(gridStartTile.x + tileRowIndex * _tileSize, gridStartTile.y + tileColumnIndex * _tileSize);
        return closestTile;
    }
}
