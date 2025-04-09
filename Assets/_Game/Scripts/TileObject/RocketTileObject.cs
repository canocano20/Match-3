public abstract class RocketTileObject : TileObject
{
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

    public abstract override void Activate();
}