using UnityEngine;

public class GridManager : MonoBehaviour
{
    public LevelData LevelData;
    public GameObject TilePrefab;
    public GameObject TileObjectPrefab;

    private Tile[,] _tiles;
    private Tile _selectedTile;
    private Swapper _swapper;
    private MatchChecker _matchChecker;

    private void Awake()
    {
        _matchChecker = new MatchChecker(this);
        _swapper = new Swapper(this, _matchChecker);
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        _tiles = new Tile[LevelData.Width, LevelData.Height];
        Vector2 centerOffset = new Vector2(
            (LevelData.Width - 1) * 0.5f,
            (LevelData.Height - 1) * 0.5f
        );

        // Create tiles
        for (int x = 0; x < LevelData.Width; x++)
        {
            for (int y = 0; y < LevelData.Height; y++)
            {
                CreateTile(x, y, centerOffset);
            }
        }

        // Set up adjacency
        SetupAdjacency();

        // Populate with tile objects
        PopulateTileObjects();
    }

    private void CreateTile(int x, int y, Vector2 centerOffset)
    {
        Vector3 position = new Vector3(x - centerOffset.x, y - centerOffset.y, 0);
        GameObject tileObj = Instantiate(TilePrefab, position, Quaternion.identity, transform);
        Tile tile = tileObj.GetComponent<Tile>();
        tile.SetPosition(x, y);
        _tiles[x, y] = tile;
    }

    private void SetupAdjacency()
    {
        for (int x = 0; x < LevelData.Width; x++)
        {
            for (int y = 0; y < LevelData.Height; y++)
            {
                Tile tile = _tiles[x, y];
                tile.Left = x > 0 ? _tiles[x - 1, y] : null;
                tile.Right = x < LevelData.Width - 1 ? _tiles[x + 1, y] : null;
                tile.Up = y < LevelData.Height - 1 ? _tiles[x, y + 1] : null;
                tile.Down = y > 0 ? _tiles[x, y - 1] : null;
            }
        }
    }

    private void PopulateTileObjects()
    {
        for (int x = 0; x < LevelData.Width; x++)
        {
            for (int y = 0; y < LevelData.Height; y++)
            {
                CreateTileObject(_tiles[x, y]);
            }
        }
    }

    private void CreateTileObject(Tile tile)
    {
        GameObject tileObjectObj = Instantiate(TileObjectPrefab, transform);
        TileObject tileObject = tileObjectObj.GetComponent<TileObject>();
        tileObject.SetType(GetRandomTileType());
        tileObject.transform.position = tile.transform.position;
        tile.SetTileObject(tileObject);
    }

    private TileObjectType GetRandomTileType()
    {
        return (TileObjectType)Random.Range(0, System.Enum.GetValues(typeof(TileObjectType)).Length);
    }

    public void SelectTile(Vector2 screenPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(screenPosition, Vector2.zero);
        _selectedTile = hit.collider?.GetComponent<Tile>();
    }

    public bool AttemptSwap(Vector2 startPos, Vector2 currentPos)
    {
        if (_selectedTile == null) return false;

        Vector2 direction = currentPos - startPos;
        Vector2Int swapDirection = GetSwapDirection(direction);
        Tile targetTile = GetTileAt(_selectedTile.X + swapDirection.x, _selectedTile.Y + swapDirection.y);

        if (targetTile == null) return false;

        return _swapper.TrySwap(_selectedTile, targetTile, isValidSwap =>
        {
            if (isValidSwap)
            {
                // Handle successful match
            }
        });
    }

    private Vector2Int GetSwapDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return new Vector2Int(direction.x > 0 ? 1 : -1, 0);
        }
        return new Vector2Int(0, direction.y > 0 ? 1 : -1);
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x >= 0 && x < LevelData.Width && y >= 0 && y < LevelData.Height)
        {
            return _tiles[x, y];
        }
        return null;
    }
}