using System;
using System.Collections.Generic;
using DG.Tweening;
using Grid;

public class Swapper
{
    private readonly GridManager _gridManager;
    private readonly MatchChecker _matchChecker;

    private float _swapAttemptDuration = 0.2f;
    private float _swapReverseDuration = 0.15f;

    public Swapper(GridManager gridManager, MatchChecker matchChecker)
    {
        _gridManager = gridManager;
        _matchChecker = matchChecker;
    }

    public bool TrySwap(Tile fromTile, Tile toTile, Action onComplete)
    {
        if (!CanSwap(fromTile, toTile))
        {
            onComplete?.Invoke();
            return false;
        }

        TileObject fromObj = fromTile.TileObject;
        TileObject toObj = toTile.TileObject;

        fromObj.transform.DOMove(toTile.transform.position, _swapAttemptDuration).SetEase(_gridManager.SwapEase);
        toObj.transform.DOMove(fromTile.transform.position, _swapAttemptDuration).SetEase(_gridManager.SwapEase)
            .OnComplete(() =>
            {
                fromTile.SetTileObject(toObj);
                toTile.SetTileObject(fromObj);

                List<Tile> matchingTiles = _matchChecker.CheckForMatches();

                if (matchingTiles.Count > 0)
                {
                    _gridManager.StartCoroutine(_gridManager.ProcessMatches(onComplete));
                }
                else
                {
                    fromTile.SetTileObject(fromObj);
                    toTile.SetTileObject(toObj);
                    fromObj.transform.DOMove(fromTile.transform.position, _swapReverseDuration).SetEase(_gridManager.SwapEase);
                    toObj.transform.DOMove(toTile.transform.position, _swapReverseDuration).SetEase(_gridManager.SwapEase)
                        .OnComplete(() => onComplete?.Invoke());
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