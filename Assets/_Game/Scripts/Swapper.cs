using DG.Tweening;
using System;

public class Swapper
{
    private readonly GridManager _gridManager;
    private readonly MatchChecker _matchChecker;

    public Swapper(GridManager gridManager, MatchChecker matchChecker)
    {
        _gridManager = gridManager;
        _matchChecker = matchChecker;
    }

    public bool TrySwap(Tile fromTile, Tile toTile, Action<bool> onComplete)
    {
        if (!CanSwap(fromTile, toTile))
        {
            onComplete?.Invoke(false);
            return false;
        }

        TileObject fromObj = fromTile.TileObject;
        TileObject toObj = toTile.TileObject;

        fromObj.transform.DOMove(toTile.transform.position, 0.2f);
        toObj.transform.DOMove(fromTile.transform.position, 0.2f)
            .OnComplete(() =>
            {
                fromTile.SetTileObject(toObj);
                toTile.SetTileObject(fromObj);

                bool isValidSwap = _matchChecker.CheckForMatches();

                if (isValidSwap)
                {
                    onComplete?.Invoke(true);
                }
                else
                {
                    fromTile.SetTileObject(fromObj);
                    toTile.SetTileObject(toObj);

                    fromObj.transform.DOMove(fromTile.transform.position, 0.2f);
                    toObj.transform.DOMove(toTile.transform.position, 0.2f)
                        .OnComplete(() => onComplete?.Invoke(false));
                }
            });

        return true;
    }

    private bool CanSwap(Tile a, Tile b)
    {
        return a != null && b != null &&
               a.IsAdjacent(b) &&
               a.HasTileObject && b.HasTileObject;
    }
}