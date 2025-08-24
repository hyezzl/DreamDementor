using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using Tilemap3D.Collections;

namespace Tilemap3D
{
    [Serializable]
    public class TileDictionary : SDictionary<TileKey, Tile>
    {
        public void Add(Tile tile)
        {
            TileKey key = new TileKey(tile);
            if (base.TryGetValue(key, out _))
                this[key] = tile;
            else
                Add(key, tile);
        }

        public bool Remove(Tile tile)
        {
            return Remove(new TileKey(tile));
        }

        public new bool TryGetValue(TileKey key, out Tile value)
        {
            if (!base.TryGetValue(key, out value))
                return false;

            if (value != null)
                return true;

            return false;
        }
    }
}
