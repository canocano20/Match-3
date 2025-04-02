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
    public Ease DropEase = Ease.OutBack;

    private Tile[,] _tiles;
    private Tile _selectedTile;
    private Swapper _swapper;
    private MatchChecker _matchChecker;

    public bool IsProcessing { get; private set; }

    [SerializeField] private float _yThreshold = 3f;
    [SerializeField] private float _animationDuration = 0.3f;
    [SerializeField] private float _animationWaitDuration = 0.15f;
    [SerializeField] private float _spawnAnimationDuration = 0.4f;
    [SerializeField] private float _spawnDelay = 0.05f;

    [SerializeField] private Vector3 _punchScaleAmount = new Vector3(0.1f, 0.1f, 0);
    [SerializeField] private float _punchScaleDuration = 0.1f;
    [SerializeField] private int _punchScaleVibrato = 10;
    [SerializeField] private float _punchScaleElasticity = 1f;

    [SerializeField] private float _baseFallDuration = 0.2f;
    [SerializeField] private float _fallDurationIncrement = 0.05f;
    [SerializeField] private float _tileObjectDestroyDuration = 0.05f;

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

        for (int x = 0; x < LevelData.Width; x++)
        {
            for (int y = 0; y < LevelData.Height; y++)
            {
                CreateTile(x, y, centerOffset);
            }
        }

        SetupAdjacency();
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
        {
            _selectedTile.PlayFailSwapAnimation(swapDirection);
            return true;
        }

        IsProcessing = true;
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

            DestroyMathingTiles(matchingTiles);

            List<Tween> shiftTweens = ShiftTilesDown();

            Sequence shiftSequence = DOTween.Sequence();
            foreach (var tween in shiftTweens)
            {
                shiftSequence.Join(tween);
            }

            //yield return shiftSequence.WaitForCompletion();
            //yield return new WaitForSeconds(_spawnDelay);

            List<Tween> spawnTweens = SpawnNewTilesAtTop();

            Sequence spawnSequence = DOTween.Sequence();
            foreach (var tween in spawnTweens)
            {
                spawnSequence.Join(tween);
            }

            yield return spawnSequence.WaitForCompletion();
            yield return new WaitForSeconds(_animationWaitDuration);
        }

        IsProcessing = false;
        onComplete?.Invoke();
    }

    private void DestroyMathingTiles(List<Tile> matchingTiles)
    {
        foreach (Tile tile in matchingTiles)
        {
            if (tile.HasTileObject)
            {
                TileObject tileObject = tile.TileObject;

                tile.SetTileObject(null);
                tileObject.SetDestroySequence(_tileObjectDestroyDuration);
            }
        }
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
                    int fallDistance = y - emptyTileY;
                    float duration = _baseFallDuration + (fallDistance - 1) * _fallDurationIncrement;
                    Tween tween = movingObject.transform.DOMove(emptyTile.transform.position, duration)
                    .SetEase(DropEase)
                    .OnComplete(() => {
                        movingObject.transform.DOPunchScale(_punchScaleAmount, _punchScaleDuration, _punchScaleVibrato, _punchScaleElasticity);
                    });
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
            int highestY = -1;
            int emptyTileY = -1;

            for (int y = 0; y < LevelData.Height; y++)
            {
                if (GetTileAt(x, y).HasTileObject)
                {
                    emptyTileY = y;
                    highestY = LevelData.Height - y;
                }
            }

            _yThreshold = highestY;

            for (int y = LevelData.Height - 1; y >= 0; y--)
            {
                Tile tile = GetTileAt(x, y);
                if (!tile.HasTileObject)
                {
                    GameObject tileObjectObj = Instantiate(TileObjectPrefab, transform);
                    TileObject tileObject = tileObjectObj.GetComponent<TileObject>();
                    tileObject.SetType(GetRandomTileType());
                    Vector3 finalPosition = tile.transform.position;
                    tileObject.transform.position = finalPosition + Vector3.up * _yThreshold;
                    int fallDistance = y - emptyTileY;
                    float duration = _spawnAnimationDuration + (fallDistance - 1) * _fallDurationIncrement;
                    Tween tween = tileObject.transform.DOMove(finalPosition, duration)
                    .SetEase(DropEase)
                    .OnComplete(() => {
                        tileObject.transform.DOPunchScale(_punchScaleAmount, _punchScaleDuration, _punchScaleVibrato, _punchScaleElasticity);
                    });
                    tweens.Add(tween);
                    tile.SetTileObject(tileObject);
                }
                else
                {
                    break;
                }
            }
        }
        return tweens;
    }
}