using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameGrid : MonoBehaviour
{
    private readonly Dictionary<Vector2, GameObject> _occupiedTiles = new();

    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private float _tileSize;

    public Vector2 GetNextTileInDirection(Vector2 positionInGrid, EDirection direction)
    {
        Vector2 newPositionInGrid = positionInGrid + direction.GetDirectionVector() * _tileSize;
        newPositionInGrid = WrapPosition(newPositionInGrid);
        return newPositionInGrid;
    }

    private Vector2 WrapPosition(Vector2 position)
    {
        if (position.x < _collider.bounds.min.x)
        {
            position.x = _collider.bounds.max.x;
        }
        else if (position.x >= _collider.bounds.max.x + _tileSize)
        {
            position.x = _collider.bounds.min.x;
        }

        if (position.y < _collider.bounds.min.y)
        {
            position.y = _collider.bounds.max.y;
        }
        else if (position.y >= _collider.bounds.max.y + _tileSize)
        {
            position.y = _collider.bounds.min.y;
        }

        return position;
    }

    public Vector2 GetRandomUnoccupiedTile(int maxNumberOfIteration = 100)
    {
        Vector2 tilePosition = GetRandomTile();
        int numberOfIterations = 0;
        while (_occupiedTiles.ContainsKey(tilePosition) && numberOfIterations < maxNumberOfIteration)
        {
            tilePosition = GetRandomTile();
            numberOfIterations++;
        }

        if (numberOfIterations == maxNumberOfIteration) Debug.LogWarning("No unoccupied tile found");
        return tilePosition;
    }

    private Vector2 GetRandomTile()
    {
        Vector2 randomPositionInGrid = new(Random.Range(_collider.bounds.min.x, _collider.bounds.max.x),
            Random.Range(_collider.bounds.min.y, _collider.bounds.max.y));
        return GetClosestTile(randomPositionInGrid);
    }

    public Vector2 GetClosestTile(Vector2 position)
    {
        Vector2 gridStartTile = _collider.bounds.min;

        float tileRowIndex = Mathf.Floor((position.x - gridStartTile.x) / _tileSize);
        float tileColumnIndex = Mathf.Floor((position.y - gridStartTile.y) / _tileSize);

        Vector2 closestTile = new(gridStartTile.x + (tileRowIndex + _tileSize) * _tileSize,
            gridStartTile.y + (tileColumnIndex + _tileSize) * _tileSize);

        return closestTile;
    }

    public void MarkTileAsOccupied(Vector2 tilePosition, GameObject occupier)
    {
        _occupiedTiles[tilePosition] = occupier;
    }

    public void MarkTileAsUnOccupied(Vector2 tilePosition)
    {
        _occupiedTiles.Remove(tilePosition);
    }

    public bool IsPositionOccupied(Vector2 tilePosition)
    {
        return _occupiedTiles.ContainsKey(tilePosition);
    }

    public GameObject GetGameObjectAtOccupiedTile(Vector2 tilePosition)
    {
        return _occupiedTiles[tilePosition];
    }

    private void OnDrawGizmosSelected()
    {
        ShowGrid();
        ShowOccupiedTiles();
    }

    private void ShowGrid()
    {
        if (_collider == null) return;
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

    private void ShowOccupiedTiles()
    {
        Gizmos.color = Color.red;
        foreach (Vector2 occupiedTilePosition in _occupiedTiles.Keys)
        {
            Gizmos.DrawCube(occupiedTilePosition, Vector2.one * _tileSize);
        }
    }
}