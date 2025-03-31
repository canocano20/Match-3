using DG.Tweening;
using System;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public TileObjectType Type { get; private set; }
    private Tile _currentTile;

    public void SetTile(Tile tile)
    {
        _currentTile = tile;
    }

    public void SetType(TileObjectType type)
    {
        Type = type;
        GetComponent<SpriteRenderer>().color = GetColorByType(type);
    }

    public void SwapWith(TileObject other, Action onComplete = null)
    {
        if (_currentTile == null || other?._currentTile == null) 
            return;

        Tile myOriginalTile = _currentTile;
        Tile otherOriginalTile = other._currentTile;

        // Swap tile references
        myOriginalTile.SetTileObject(other);
        otherOriginalTile.SetTileObject(this);

        // Animate movement
        transform.DOMove(otherOriginalTile.transform.position, 0.2f)
            .OnComplete(() => onComplete?.Invoke());
        other.transform.DOMove(myOriginalTile.transform.position, 0.2f);
    }

    private Color GetColorByType(TileObjectType type)
    {
        return type switch
        {
            TileObjectType.Red => Color.red,
            TileObjectType.Blue => Color.blue,
            TileObjectType.Green => Color.green,
            TileObjectType.Yellow => Color.yellow,
            _ => Color.white
        };
    }
}