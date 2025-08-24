using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Tilemap3D
{
    [Serializable]
    public class TileContext
    {
        public Tile[] neighbors;

        public Tile GetNeighborTile(int matchGridIndex) => GetNeighborTile(neighbors, matchGridIndex);
        public static Tile GetNeighborTile(Tile[] neighbors, int matchGridIndex)
        {
            return neighbors[matchGridIndex];
        }

        public void SetNeighborTile(int matchGridIndex, Tile tile) => SetNeighborTile(neighbors, matchGridIndex, tile);
        public static void SetNeighborTile(Tile[] neighbors, int matchGridIndex, Tile tile)
        {
            neighbors[matchGridIndex] = tile;
        }

        public Tile GetNeighborTile(Vector3Int neighborCellOffset)
        {
            if (neighborCellOffset == Vector3Int.zero)
                return neighbors[4];
            else if (neighborCellOffset == Vector3Int.forward)
                return neighbors[1];
            else if (neighborCellOffset == Vector3Int.back)
                return neighbors[7];
            else if (neighborCellOffset == Vector3Int.right)
                return neighbors[5];
            else if (neighborCellOffset == Vector3Int.left)
                return neighbors[3];
            else if (neighborCellOffset == (Vector3Int.right + Vector3Int.forward))
                return neighbors[2];
            else if (neighborCellOffset == (Vector3Int.right + Vector3Int.back))
                return neighbors[8];
            else if (neighborCellOffset == (Vector3Int.left + Vector3Int.forward))
                return neighbors[0];
            else if (neighborCellOffset == (Vector3Int.left + Vector3Int.back))
                return neighbors[6];
            else
                return null;
        }
    }
}
