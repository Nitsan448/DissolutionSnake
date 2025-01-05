using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGridMarker : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    private GameGrid _gameGrid;

    public void Init(GameGrid gameGrid)
    {
        _gameGrid = gameGrid;
    }

    private void Start()
    {
        MarkAllTilesInGrid();
    }

    private void MarkAllTilesInGrid()
    {
        for (int column = _tilemap.cellBounds.xMin; column < _tilemap.cellBounds.xMax; column++)
        {
            for (int row = _tilemap.cellBounds.yMin; row < _tilemap.cellBounds.yMax; row++)
            {
                Vector3Int cellPosition = new(column, row, 0);
                TileBase tile = _tilemap.GetTile(cellPosition);

                if (tile == null) continue;

                Vector3 worldPosition = _tilemap.CellToWorld(cellPosition);
                Vector2 tilePositionInGrid = _gameGrid.GetClosestTile(worldPosition);
                _gameGrid.MarkTileAsOccupied(tilePositionInGrid, gameObject);
            }
        }
    }
}