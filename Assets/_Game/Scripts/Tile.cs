using DG.Tweening;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int X, Y;
    public Tile Left, Right, Up, Down;
    public TileObject TileObject { get; private set; }
    public bool HasTileObject => TileObject != null;

    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SetTileObject(TileObject tileObject)
    {
        TileObject = tileObject;
    }

    public bool IsAdjacent(Tile other)
    {
        return other != null &&
              (other == Left || other == Right ||
               other == Up || other == Down);
    }

    public void PlayFailSwapAnimation(Vector2Int swapDirection)
    {
        if (HasTileObject)
        {
            if (DOTween.IsTweening(TileObject.transform))
                return;

            Vector3 moveVector = (new Vector3(swapDirection.x, swapDirection.y) * 0.1f) + TileObject.transform.position;
            TileObject.transform.DOMove(moveVector, 0.15f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);
        }
    }
}