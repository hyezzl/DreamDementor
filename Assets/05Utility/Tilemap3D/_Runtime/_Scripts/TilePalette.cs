using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityObject = UnityEngine.Object;

namespace Tilemap3D
{
    [CreateAssetMenu(fileName = "MyTilePalette", menuName = "Tilemap3D/TilePalette")]
    public class TilePalette : ScriptableObject
    {
        [SerializeField] private List<UnityObject> tiles = new List<UnityObject>();

        private const string WARNING_INVALID_PALETTE_TILE = "Warning: Detected invalid palette tile, removing it. You can only add prefabs or assets that " +
                                                            "are not null and that implement '" + nameof(IPaletteTile) + "' to a tile palette.";

        private void OnValidate()
        {
            while (tiles != null && tiles.Count > 0)
            {
                IPaletteTile paletteTile;
                if (tiles[tiles.Count - 1] is GameObject tilePrefab)
                    paletteTile = tilePrefab.GetComponent<Tile>();
                else
                    paletteTile = tiles[tiles.Count - 1] as IPaletteTile;

                if (paletteTile == null)
                {
                    tiles.RemoveAt(tiles.Count - 1);
                    Debug.LogWarning(WARNING_INVALID_PALETTE_TILE, this);
                }
                else
                    break;
            }
        }

        public List<IPaletteTile> GetTiles()
        {
            List<IPaletteTile> paletteTiles = new List<IPaletteTile>();

            if (tiles == null)
                return paletteTiles;

            for (int i = 0; i < tiles.Count; i++)
            {
                IPaletteTile paletteTile;
                if (tiles[i] is GameObject tilePrefab)
                    paletteTile = tilePrefab.GetComponent<Tile>();
                else
                    paletteTile = tiles[i] as IPaletteTile;

                if (paletteTile == null)
                {
                    tiles.RemoveAt(i--);
                    Debug.LogWarning(WARNING_INVALID_PALETTE_TILE, this);
                    continue;
                }

                paletteTiles.Add(paletteTile);
            }

            return paletteTiles;
        }
    }
}