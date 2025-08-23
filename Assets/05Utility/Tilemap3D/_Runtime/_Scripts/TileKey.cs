using UnityEngine;

using System;

namespace Tilemap3D
{
    [Serializable]
    public class TileKey : IEquatable<TileKey>
    {
        public Vector3Int cellPosition;
        public TileLayer tileLayer;

        public TileKey(Tile tile) : this(tile.GridCellPosition, tile.Layer) { }
        public TileKey(Vector3Int cellPosition, TileLayer tileLayer)
        {
            this.cellPosition = cellPosition;
            this.tileLayer = tileLayer;
        }

        public bool Equals(TileKey other)
        {
            return cellPosition.Equals(other.cellPosition) && Equals(tileLayer, other.tileLayer);
        }

        public override bool Equals(object obj)
        {
            return obj is TileKey other && obj.Equals(other);
        }

        public override int GetHashCode() 
        {
            return HashCode.Combine(cellPosition, tileLayer);
        }

        public override string ToString()
        {
            return $"({cellPosition}, {tileLayer.name})";
        }
    }
}
