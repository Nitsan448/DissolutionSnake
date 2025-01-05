using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePositionsGetter : MonoBehaviour
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
        BoundsInt bounds = _tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                Vector3 worldPosition = _tilemap.CellToWorld(cellPosition);
                Vector2 tilePositionInGrid = _gameGrid.GetClosestTile(worldPosition);
                TileBase tile = _tilemap.GetTile(cellPosition);

                if (tile == null) continue;
                _gameGrid.MarkTileAsOccupied(tilePositionInGrid, gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        GameGrid gameGrid = FindFirstObjectByType<GameGrid>();

        BoundsInt bounds = _tilemap.cellBounds;

        Gizmos.color = Color.red;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                Vector3 worldPosition = _tilemap.CellToWorld(cellPosition);
                Vector2 tilePositionInGrid = gameGrid.GetClosestTile(worldPosition);

                TileBase tile = _tilemap.GetTile(cellPosition);

                // Draw grid tile position
                if (tile != null)
                {
                    Gizmos.DrawCube(tilePositionInGrid, Vector3.one * gameGrid.TileSize * 0.9f);
                }
            }
        }
    }
}