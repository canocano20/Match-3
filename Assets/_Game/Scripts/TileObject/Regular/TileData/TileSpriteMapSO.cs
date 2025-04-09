using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Tiles/Tile Sprite Map")]
public class TileSpriteMapSO : ScriptableObject
{
    [Serializable]
    public class TileSpriteEntry
    {
        public string TypeName;
        public Sprite Sprite;
    }

    public List<TileSpriteEntry> Entries;

    private Dictionary<Type, Sprite> _spriteMap;

    public Dictionary<Type, Sprite> GetMap()
    {
        if (_spriteMap == null)
        {
            _spriteMap = new Dictionary<Type, Sprite>();
            foreach (var entry in Entries)
            {
                Type type = Type.GetType(entry.TypeName);
                if (type != null)
                {
                    _spriteMap[type] = entry.Sprite;
                }
                else
                {
                    Debug.LogWarning($"Type not found for {entry.TypeName}");
                }
            }
        }

        return _spriteMap;
    }
}
