using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapMarkerOnGrid : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;

    private void Start()
    {
        List<Vector2> tilePositionsInGrid = GetTilePositionsInGrid();
        MarkTilesInGrid(tilePositionsInGrid);
    }


    private List<Vector2> GetTilePositionsInGrid()
    {
        List<Vector2> tilePositionsInGrid = new List<Vector2>();

        for (int column = _tilemap.cellBounds.xMin; column < _tilemap.cellBounds.xMax; column++)
        {
            for (int row = _tilemap.cellBounds.yMin; row < _tilemap.cellBounds.yMax; row++)
            {
                Vector3Int cellPosition = new(column, row, 0);
                TileBase tile = _tilemap.GetTile(cellPosition);

                if (tile == null) continue;

                Vector3 worldPosition = _tilemap.CellToWorld(cellPosition);
                Vector2 tilePositionInGrid = GameManager.Instance.GameGrid.GetClosestTile(worldPosition);
                tilePositionsInGrid.Add(tilePositionInGrid);
            }
        }

        return tilePositionsInGrid;
    }

    private void MarkTilesInGrid(List<Vector2> tilePositionsInGrid)
    {
        foreach (Vector2 tilePosition in tilePositionsInGrid)
        {
            GameManager.Instance.GameGrid.MarkTileAsOccupied(tilePosition, gameObject);
        }
    }
}