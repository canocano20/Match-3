using Grid;
using UnityEngine;

public class ColorRemovingTileObject : TileObject
{
    public override bool IsSpecial => true;
    public override bool IsMatchable => false;
    public override bool Matches(TileObject other) => false;

    public override void Initialize(Tile tile)
    {
        base.Initialize(tile);
    }

    public override void OnSwap(Tile otherTile)
    {
        Activate();
    }

    public override void OnClick()
    {
        Activate();
    }

    public override void Activate()
    {
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        for (int x = 0; x < gridManager.LevelData.Width; x++)
        {
            for (int y = 0; y < gridManager.LevelData.Height; y++)
            {
                Tile target = gridManager.GetTileAt(x, y);
                if (target != null && target.HasTileObject &&
                    target.TileObject is RegularTileObject regular)
                {
                    target.TileObject.DestroyTile();
                    target.SetTileObject(null);
                }
            }
        }
        DestroyTile();
    }
}