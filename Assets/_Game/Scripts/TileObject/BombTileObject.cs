using Grid;
using UnityEngine;

public class BombTileObject : TileObject
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

        int x = Tile.X;
        int y = Tile.Y;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Tile target = gridManager.GetTileAt(x + dx, y + dy);
                if (target != null && target.HasTileObject)
                {
                    target.TileObject.DestroyTile();
                    target.SetTileObject(null);
                }
            }
        }
        DestroyTile();
    }
}