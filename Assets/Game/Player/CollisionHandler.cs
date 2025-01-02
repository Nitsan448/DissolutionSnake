using UnityEngine;

public class CollisionHandler
{
    private LayerMask _obstaclesLayerMask;
    private Player _player;

    public CollisionHandler(LayerMask obstaclesLayerMask, Player player)
    {
        _obstaclesLayerMask = obstaclesLayerMask;
        _player = player;
    }

    public void HandleCollisionsInNextTile(Vector3 headPosition, Vector3 nextTilePosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(headPosition, nextTilePosition - headPosition);
        if (hit)
        {
            if (((1 << hit.transform.gameObject.layer) & _obstaclesLayerMask) != 0)
            {
                //Hit wall
                _player.HitObstacle();
            }
            else if (hit.transform.gameObject.TryGetComponent(out Item item))
            {
                //Hit item
                _player.HitItem(hit.transform.gameObject);
            }
        }
    }
}