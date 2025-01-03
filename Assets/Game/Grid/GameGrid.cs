using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameGrid : MonoBehaviour
{
    //I am not saving the tiles in a data structure to save memory.
    public float TileSize => _tileSize;
    private readonly Dictionary<Vector2, GameObject> _occupiedTiles = new();

    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private float _tileSize;

    public Vector2 GetNextTileInDirection(Vector2 positionInGrid, EDirection direction)
    {
        Vector2 newPositionInGrid = positionInGrid + direction.GetDirectionVector() * _tileSize;
        return newPositionInGrid;
    }

    public Vector2 GetRandomUnoccupiedTile()
    {
        Vector2 tilePosition = GetRandomTile();
        while (_occupiedTiles.ContainsKey(tilePosition))
        {
            Debug.Log(tilePosition);
            tilePosition = GetRandomTile();
        }

        return tilePosition;
    }

    private Vector2 GetRandomTile()
    {
        Vector2 randomPositionInGrid = new(Random.Range(_collider.bounds.min.x, _collider.bounds.max.x),
            Random.Range(_collider.bounds.min.y, _collider.bounds.max.y));
        return GetClosestTile(randomPositionInGrid);
    }

    public Vector2 GetClosestTile(Vector2 positionInGrid)
    {
        //TODO: make this work with a position outside of the grid
        Vector2 gridStartTile = _collider.bounds.min;

        float tileRowIndex = Mathf.Round((positionInGrid.x - gridStartTile.x) / _tileSize);
        float tileColumnIndex = Mathf.Round((positionInGrid.y - gridStartTile.y) / _tileSize);

        Vector2 closestTile = new(gridStartTile.x + tileRowIndex * _tileSize, gridStartTile.y + tileColumnIndex * _tileSize);
        return closestTile;
    }

    public void MarkTileAsOccupied(Vector2 tilePosition, GameObject occupier)
    {
        _occupiedTiles.Add(tilePosition, occupier);
    }

    public void MarkTileAsUnOccupied(Vector2 tilePosition)
    {
        _occupiedTiles.Remove(tilePosition);
    }


    private void OnDrawGizmosSelected()
    {
        ShowGrid();
    }

    private void ShowGrid()
    {
        //TODO: refactor
        Gizmos.color = Color.cyan;
        int numberOfRows = (int)(Mathf.Round(_collider.bounds.max.y - _collider.bounds.min.y) / _tileSize);
        int numberOfColumns = (int)(Mathf.Round(_collider.bounds.max.x - _collider.bounds.min.x) / _tileSize);
        for (int i = 0; i < numberOfRows; i++)
        {
            float rowYPosition = _collider.bounds.min.y + i * _tileSize + _tileSize / 2;
            Vector2 lineStart = new(_collider.bounds.min.x, rowYPosition);
            Vector2 lineEnd = new(_collider.bounds.max.x, rowYPosition);
            Gizmos.DrawLine(lineStart, lineEnd);
        }

        for (int i = 0; i < numberOfColumns; i++)
        {
            float rowXPosition = _collider.bounds.min.x + i * _tileSize + _tileSize / 2;
            Vector2 lineStart = new(rowXPosition, _collider.bounds.min.y);
            Vector2 lineEnd = new(rowXPosition, _collider.bounds.max.y);
            Gizmos.DrawLine(lineStart, lineEnd);
        }
    }
}