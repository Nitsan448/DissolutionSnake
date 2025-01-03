using System;
using System.Collections.Generic;
using UnityEngine;

public class SnakeNode : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _headSprite;
    [SerializeField] private Sprite _segmentSprite;


    public void MakeHead()
    {
        _spriteRenderer.sprite = _headSprite;
    }

    public void MakeBody()
    {
        _spriteRenderer.sprite = _segmentSprite;
    }
}