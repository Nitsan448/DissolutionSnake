using UnityEngine;

public class CollisionHandler
{
    private Player _player;
    private GameGrid _gameGrid;
    private readonly LayerMask _obstaclesLayerMask;

    public CollisionHandler(Player player, LayerMask obstaclesLayerMask)
    {
        _player = player;
        _obstaclesLayerMask = obstaclesLayerMask;
    }

    public void HandleCollisionsInNextTile(Vector2 nextTilePosition, Vector2 snakeHeadPosition, float gridTileSize)
    {
        if (Object.FindFirstObjectByType<GameGrid>().IsPositionOccupied(nextTilePosition))
        {
            GameObject gameObject = Object.FindFirstObjectByType<GameGrid>().GetGameObjectAtOccupiedTile(nextTilePosition);
            HandleCollision(gameObject);
        }
    }

    private void HandleCollision(GameObject hit)
    {
        if (((1 << hit.layer) & _obstaclesLayerMask) != 0)
        {
            _player.HitObstacle(hit);
        }
        else if (hit.TryGetComponent(out Item item))
        {
            _player.HitItem(item);
        }
    }
}