public class MatchChecker
{
    private readonly GridManager _gridManager;

    public MatchChecker(GridManager gridManager)
    {
        _gridManager = gridManager;
    }

    public bool CheckForMatches()
    {
        return CheckHorizontalMatches() || CheckVerticalMatches();
    }

    private bool CheckHorizontalMatches()
    {
        for (int y = 0; y < _gridManager.LevelData.Height; y++)
        {
            int matchCount = 1;
            TileObjectType? prevType = null;
            for (int x = 0; x < _gridManager.LevelData.Width; x++)
            {
                Tile tile = _gridManager.GetTileAt(x, y);
                if (tile == null || !tile.HasTileObject)
                {
                    matchCount = 1;
                    prevType = null;
                    continue;
                }
                TileObjectType currentType = tile.TileObject.Type;
                if (currentType == prevType)
                {
                    matchCount++;
                    if (matchCount >= 3)
                    {
                        return true;
                    }
                }
                else
                {
                    matchCount = 1;
                    prevType = currentType;
                }
            }
        }
        return false;
    }

    private bool CheckVerticalMatches()
    {
        for (int x = 0; x < _gridManager.LevelData.Width; x++)
        {
            int matchCount = 1;
            TileObjectType? prevType = null;
            for (int y = 0; y < _gridManager.LevelData.Height; y++)
            {
                Tile tile = _gridManager.GetTileAt(x, y);
                if (tile == null || !tile.HasTileObject)
                {
                    matchCount = 1;
                    prevType = null;
                    continue;
                }
                TileObjectType currentType = tile.TileObject.Type;
                if (currentType == prevType)
                {
                    matchCount++;
                    if (matchCount >= 3)
                    {
                        return true;
                    }
                }
                else
                {
                    matchCount = 1;
                    prevType = currentType;
                }
            }
        }
        return false;
    }
}