using Grid;
using System.Collections.Generic;

public class MatchChecker
{
    private readonly GridManager _gridManager;

    public MatchChecker(GridManager gridManager)
    {
        _gridManager = gridManager;
    }

    public List<Tile> CheckForMatches()
    {
        List<Tile> matchingTiles = new List<Tile>();
        matchingTiles.AddRange(CheckHorizontalMatches());
        matchingTiles.AddRange(CheckVerticalMatches());
        return matchingTiles;
    }

    private List<Tile> CheckHorizontalMatches()
    {
        List<Tile> matchingTiles = new List<Tile>();
        for (int y = 0; y < _gridManager.LevelData.Height; y++)
        {
            int matchCount = 1;
            TileObject prevTileObject = null;
            List<Tile> currentMatch = new List<Tile>();

            for (int x = 0; x < _gridManager.LevelData.Width; x++)
            {
                Tile tile = _gridManager.GetTileAt(x, y);
                if (tile == null || !tile.HasTileObject || !tile.TileObject.IsMatchable)
                {
                    if (matchCount >= 3) matchingTiles.AddRange(currentMatch);
                    matchCount = 1;
                    prevTileObject = null;
                    currentMatch.Clear();
                    continue;
                }

                TileObject currentTileObject = tile.TileObject;
                if (prevTileObject != null && currentTileObject.Matches(prevTileObject))
                {
                    matchCount++;
                    currentMatch.Add(tile);
                }
                else
                {
                    if (matchCount >= 3) matchingTiles.AddRange(currentMatch);
                    matchCount = 1;
                    prevTileObject = currentTileObject;
                    currentMatch.Clear();
                    currentMatch.Add(tile);
                }
            }
            if (matchCount >= 3) matchingTiles.AddRange(currentMatch);
        }
        return matchingTiles;
    }

    private List<Tile> CheckVerticalMatches()
    {
        List<Tile> matchingTiles = new List<Tile>();
        for (int x = 0; x < _gridManager.LevelData.Width; x++)
        {
            int matchCount = 1;
            TileObject prevTileObject = null;
            List<Tile> currentMatch = new List<Tile>();

            for (int y = 0; y < _gridManager.LevelData.Height; y++)
            {
                Tile tile = _gridManager.GetTileAt(x, y);
                if (tile == null || !tile.HasTileObject || !tile.TileObject.IsMatchable)
                {
                    if (matchCount >= 3) matchingTiles.AddRange(currentMatch);
                    matchCount = 1;
                    prevTileObject = null;
                    currentMatch.Clear();
                    continue;
                }

                TileObject currentTileObject = tile.TileObject;
                if (prevTileObject != null && currentTileObject.Matches(prevTileObject))
                {
                    matchCount++;
                    currentMatch.Add(tile);
                }
                else
                {
                    if (matchCount >= 3) matchingTiles.AddRange(currentMatch);
                    matchCount = 1;
                    prevTileObject = currentTileObject;
                    currentMatch.Clear();
                    currentMatch.Add(tile);
                }
            }
            if (matchCount >= 3) matchingTiles.AddRange(currentMatch);
        }
        return matchingTiles;
    }
}