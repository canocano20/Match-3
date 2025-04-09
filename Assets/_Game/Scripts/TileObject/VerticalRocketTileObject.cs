using Grid;
using UnityEngine;

public class VerticalRocketTileObject : RocketTileObject
{
    public override bool IsSpecial => true;

    public override bool IsMatchable => false;

    public override bool Matches(TileObject other) => false;

    public override void Initialize(Tile tile)
    {
        base.Initialize(tile);
    }

    public override void Activate()
    {
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        int x = Tile.X;
        for (int y = 0; y < gridManager.LevelData.Height; y++)
        {
            Tile target = gridManager.GetTileAt(x, y);
            if (target != null && target.HasTileObject)
            {
                target.TileObject.DestroyTile();
                target.SetTileObject(null);
            }
        }
        DestroyTile();
    }
}