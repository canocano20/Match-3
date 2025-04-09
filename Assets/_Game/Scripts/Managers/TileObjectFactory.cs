
namespace Grid
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class TileObjectFactory
    {
        private static readonly Type[] RegularTileTypes = {
            typeof(AppleTileObject),
            typeof(BananaTileObject),
            typeof(BlueberryTileObject),
            typeof(GrapeTileObject),
            typeof(OrangeTileObject),
            typeof(PearTileObject),
            typeof(StrawberryTileObject)
        };

        private static readonly Type BombTile = typeof(BombTileObject);
        private static readonly Type ColorRemovingTile = typeof(ColorRemovingTileObject);
        private static readonly Type HorizontalRocketTile = typeof(HorizontalRocketTileObject);
        private static readonly Type VerticalRocketTile = typeof(VerticalRocketTileObject);

        public static TileObject CreateTileObject(GameObject tileObject, Dictionary<Type, Sprite> spriteMap)
        {
            TileObject existing = tileObject.GetComponent<TileObject>();
            if (existing != null) UnityEngine.Object.Destroy(existing);

            float rand = UnityEngine.Random.value;
            Type tileType;
            if (rand < 0.04f) tileType = typeof(HorizontalRocketTileObject);
            else if (rand < 0.08f) tileType = typeof(VerticalRocketTileObject);
            else if (rand < 0.11f) tileType = typeof(BombTileObject);
            else if (rand < 0.14f) tileType = typeof(ColorRemovingTileObject);
            else tileType = RegularTileTypes[UnityEngine.Random.Range(0, RegularTileTypes.Length)];

            TileObject tileObj = (TileObject)tileObject.AddComponent(tileType);

            if (spriteMap.TryGetValue(tileType, out Sprite sprite))
            {
                var sr = tileObject.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = sprite;
                }
            }
            else
            {
                Debug.LogWarning($"No sprite found for type {tileType.Name}");
            }

            return tileObj;
        }

    }
}