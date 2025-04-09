
using System;

[Serializable]
public class RegularTileObject : TileObject
{
    public override bool IsSpecial => false;

    public override bool IsMatchable => true;

    public override void OnSwap(Tile otherTile)
    {
        throw new System.NotImplementedException();
    }

    public override bool Matches(TileObject other)
    {
        if (other is RegularTileObject regular)
        {
            return other != null && this.GetType() == other.GetType();
        }
        return false;
    }
}