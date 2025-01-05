using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGridMarker : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Tilemap _tilemap;
    private GameGrid _gameGrid;

    public void Init(GameGrid gameGrid)
    {
        _gameGrid = gameGrid;
    }

    private void Start()
    {
        List<Vector2> tilePositionsInGrid = GetTilePositionsInGrid();
        MarkTilesInGrid(tilePositionsInGrid);
        DataPersistenceManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        DataPersistenceManager.Instance.Unregister(this);
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
                Vector2 tilePositionInGrid = _gameGrid.GetClosestTile(worldPosition);
                tilePositionsInGrid.Add(tilePositionInGrid);
            }
        }

        return tilePositionsInGrid;
    }

    private void MarkTilesInGrid(List<Vector2> tilePositionsInGrid)
    {
        foreach (Vector2 tilePosition in tilePositionsInGrid)
        {
            _gameGrid.MarkTileAsOccupied(tilePosition, gameObject);
        }
    }

    public void SaveData(GameData dataToSave)
    {
        dataToSave.TilePositions = GetTilePositionsInGrid();
    }

    public void LoadData(GameData loadedData)
    {
        MarkTilesInGrid(loadedData.TilePositions);
    }
}