using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public LevelData LevelData;
    public GameObject TilePrefab;
    public GameObject TileObjectPrefab;
    public Ease SwapEase = Ease.OutQuart;

    private Tile[,] _tiles;
    private Tile _selectedTile;
    private Swapper _swapper;
    private MatchChecker _matchChecker;

    public bool IsProcessing { get; private set; }

    [SerializeField] private float _yThreshold = 3f;
    [SerializeField] private float _animationDuration = 0.3f;

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
        // Set initial position above the final position
        Vector3 finalPosition = tile.transform.position;
        tileObject.transform.position = finalPosition + Vector3.up * _yThreshold; // Start 2 units above
        // Animate to the final position
        tileObject.transform.DOMove(finalPosition, _animationDuration);
        tile.SetTileObject(tileObject);
    }

    private TileObjectType GetRandomTileType()
    {
        return (TileObjectType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(TileObjectType)).Length);
    }

    public void SelectTile(Vector2 screenPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(screenPosition, Vector2.zero);
        _selectedTile = hit.collider?.GetComponent<Tile>();
    }

    public bool AttemptSwap(Vector2 startPos, Vector2 currentPos)
    {
        if (_selectedTile == null) 
            return false;

        Vector2 direction = currentPos - startPos;
        Vector2Int swapDirection = GetSwapDirection(direction);
        Tile targetTile = GetTileAt(_selectedTile.X + swapDirection.x, _selectedTile.Y + swapDirection.y);

        if (targetTile == null) 
            return false;

        IsProcessing = true; // Disable input at the start of the swap
        return _swapper.TrySwap(_selectedTile, targetTile, () => IsProcessing = false);
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

    public IEnumerator ProcessMatches(Action onComplete)
    {
        while (true)
        {
            List<Tile> matchingTiles = _matchChecker.CheckForMatches();
            if (matchingTiles.Count == 0)
            {
                break;
            }

            foreach (Tile tile in matchingTiles)
            {
                if (tile.HasTileObject)
                {
                    Destroy(tile.TileObject.gameObject);
                    tile.SetTileObject(null);
                }
            }

            // Shift existing tiles down
            List<Tween> shiftTweens = ShiftTilesDown();

            // Spawn new tiles at the top
            List<Tween> spawnTweens = SpawnNewTilesAtTop();

            // Combine all animations into a sequence
            Sequence sequence = DOTween.Sequence();
            foreach (var tween in shiftTweens)
            {
                sequence.Join(tween); // Run shift animations in parallel
            }
            foreach (var tween in spawnTweens)
            {
                sequence.Join(tween); // Run spawn animations in parallel
            }

            // Wait for all animations to complete
            yield return sequence.WaitForCompletion();
        }

        // Re-enable input and signal completion
        IsProcessing = false;
        onComplete?.Invoke();
    }


    private List<Tween> ShiftTilesDown()
    {
        List<Tween> tweens = new List<Tween>();
        for (int x = 0; x < LevelData.Width; x++)
        {
            int emptyTileY = -1;
            for (int y = 0; y < LevelData.Height; y++)
            {
                Tile currentTile = GetTileAt(x, y);
                if (!currentTile.HasTileObject)
                {
                    if (emptyTileY == -1)
                    {
                        emptyTileY = y;
                    }
                }
                else if (emptyTileY != -1)
                {
                    Tile emptyTile = GetTileAt(x, emptyTileY);
                    TileObject movingObject = currentTile.TileObject;
                    emptyTile.SetTileObject(movingObject);
                    currentTile.SetTileObject(null);
                    Tween tween = movingObject.transform.DOMove(emptyTile.transform.position, _animationDuration);
                    tweens.Add(tween);
                    emptyTileY++;
                }
            }
        }
        return tweens;
    }

    private List<Tween> SpawnNewTilesAtTop()
    {
        List<Tween> tweens = new List<Tween>();
        for (int x = 0; x < LevelData.Width; x++)
        {
            for (int y = LevelData.Height - 1; y >= 0; y--)
            {
                Tile tile = GetTileAt(x, y);
                if (!tile.HasTileObject)
                {
                    GameObject tileObjectObj = Instantiate(TileObjectPrefab, transform);
                    TileObject tileObject = tileObjectObj.GetComponent<TileObject>();
                    tileObject.SetType(GetRandomTileType());
                    Vector3 finalPosition = tile.transform.position;
                    tileObject.transform.position = finalPosition + Vector3.up * _yThreshold; // Start above grid
                    Tween tween = tileObject.transform.DOMove(finalPosition, _animationDuration);
                    tweens.Add(tween);
                    tile.SetTileObject(tileObject);
                }
                else
                {
                    break; // Stop when we hit an existing tile
                }
            }
        }
        return tweens;
    }
}