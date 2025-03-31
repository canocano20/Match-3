using UnityEngine;

public class Tile : MonoBehaviour
{
    public int X, Y;
    public Tile Left, Right, Up, Down;
    public TileObject TileObject { get; private set; }
    public bool HasTileObject => TileObject != null;

    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SetTileObject(TileObject tileObject)
    {
        TileObject = tileObject;

        if (tileObject != null)
            tileObject.SetTile(this);
    }

    public bool IsAdjacent(Tile other)
    {
        return other != null &&
              (other == Left || other == Right ||
               other == Up || other == Down);
    }
}