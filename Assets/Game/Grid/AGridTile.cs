using System;
using UnityEngine;

public abstract class AGridTile : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.GameGrid.MarkTileAsOccupied(transform.position, gameObject);
    }

    private void OnDisable()
    {
        GameManager.Instance.GameGrid.MarkTileAsUnOccupied(transform.position);
    }
}