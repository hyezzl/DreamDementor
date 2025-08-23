using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Tilemap3D
{
    [AddComponentMenu("Tilemap3D/Tilemap")] 
    public class Tilemap : MonoBehaviour, IEnumerable<KeyValuePair<TileKey, Tile>>
    {
        [SerializeField] private Vector3 cellSize = Vector3.one;
        [SerializeField] private Vector3 cellGap;
        [SerializeField] private ECellLayout cellLayout;
        
        [SerializeField, HideInInspector] private TileDictionary tilesDictionary = new TileDictionary();

        public enum ECellLayout { Rectangle/*, Hexagon*/ }

        public Tile GetTile(Vector3Int cellPosition, TileLayer tileLayer)
        {
            return GetTile(new TileKey(cellPosition, tileLayer));
        }
        public Tile GetTile(TileKey key)
        {
            if (tilesDictionary.TryGetValue(key, out Tile value))
            {
                return value;
            }

            return null;
        }

        public bool TryGetTile(Vector3Int cellPosition, TileLayer tileLayer, out Tile tile)
        {
            return TryGetTile(new TileKey(cellPosition, tileLayer), out tile);
        }
        public bool TryGetTile(TileKey key, out Tile tile)
        {
            return tilesDictionary.TryGetValue(key, out tile);
        }

        public bool TryAddTile(Vector3Int cellPosition, TileLayer tileLayer, Tile tile)
        {
            return TryAddTile(new TileKey(cellPosition, tileLayer), tile);
        }
        public bool TryAddTile(TileKey tileKey, Tile tile)
        {
            bool added = tilesDictionary.TryAdd(tileKey, tile);

            if (added)
                tile.Tilemap = this;

            return added;
        }

        public void SetTile(Vector3Int cellPosition, TileLayer tileLayer, Tile tile)
        {
            SetTile(new TileKey(cellPosition, tileLayer), tile);
        }
        public void SetTile(TileKey tileKey, Tile tile)
        {
            if (tilesDictionary.ContainsKey(tileKey))
                Remove(tileKey);

            tilesDictionary.Add(tileKey, tile);
            tile.Tilemap = this;
            tile.Layer = tileKey.tileLayer;
            tile.GridCellPosition = tileKey.cellPosition;
        }

        public bool Remove(Vector3Int cellPosition, TileLayer tileLayer, bool destroy = true)
        {
            return Remove(new TileKey(cellPosition, tileLayer), destroy);
        }
        public bool Remove(TileKey tileKey, bool destroy = true)
        {
            if (tilesDictionary.TryGetValue(tileKey, out Tile tile) && tile != null)
            {
                // important! otherwise tile will try to call this function again when destroyed.
                tile.Tilemap = null;

                if (destroy)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying && Application.isEditor)
                        DestroyImmediate(tile.gameObject);
                    else
                        Destroy(tile.gameObject);
#else
                    Destroy(tile.gameObject);
#endif
                }
            }

            return tilesDictionary.Remove(tileKey);
        }

        /// <returns>The cell position in the grid that is nearest to the given <paramref name="worldPosition"/>.</returns>
        public Vector3Int GetNearestGridCellPosition(Vector3 worldPosition)
        {
            Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

            Vector3Int gridCellPosition = Vector3Int.zero;

            float xn = (cellSize.x + cellGap.x);
            float yn = (cellSize.y + cellGap.y);
            float zn = (cellSize.z + cellGap.z);

            gridCellPosition.x = xn == 0 ? 0 : Mathf.RoundToInt(localPosition.x / xn);
            gridCellPosition.y = yn == 0 ? 0 : Mathf.RoundToInt(localPosition.y / yn);
            gridCellPosition.z = zn == 0 ? 0 : Mathf.RoundToInt(localPosition.z / zn);

            return gridCellPosition;
        }

        /// <returns>The world position of the given <paramref name="gridCellPosition"/>.</returns>
        public Vector3 ConvertToVector3Position(Vector3Int gridCellPosition)
        {
            Vector3 worldPosition = transform.position;

            float xn = (cellSize.x + cellGap.x) * transform.lossyScale.x;
            float yn = (cellSize.y + cellGap.y) * transform.lossyScale.y;
            float zn = (cellSize.z + cellGap.z) * transform.lossyScale.z;

            Vector3 positionOffset = transform.rotation * new Vector3(gridCellPosition.x * xn, gridCellPosition.y * yn, gridCellPosition.z * zn);

            worldPosition += positionOffset;

            return worldPosition;
        }

        public Vector3 CellSize { get => cellSize; set => cellSize = value; }
        public Vector3 CellGap { get => cellGap; set => cellGap = value; }
        public ECellLayout CellLayout { get => cellLayout; set => cellLayout = value; }

#region IEnumerable<KeyValuePair<TileKey, Tile>>
        public IEnumerator<KeyValuePair<TileKey, Tile>> GetEnumerator()
        {
            return tilesDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
#endregion

#if UNITY_EDITOR
#region Gizmos
        [NonSerialized] public bool shouldDrawGizmos;
        [NonSerialized] public int gizmoDrawStyle;
        [NonSerialized] public int gizmoGridSizeX;
        [NonSerialized] public int gizmoGridSizeY;
        [NonSerialized] public Color gizmoGridColor;
        [NonSerialized] public float gizmoGridPositionY;
        [NonSerialized] public Vector3 gizmoGridViewBias;
        
        void OnDrawGizmos()
        {
            if (!shouldDrawGizmos)
                return;

            if (gizmoGridSizeX <= 1 && gizmoGridSizeY <= 1)
                return;

            Gizmos.color = gizmoGridColor;

            int gridExtentOddX = gizmoGridSizeX % 2 == 0 ? gizmoGridSizeX + 1 : gizmoGridSizeX;
            int gridExtentOddZ = gizmoGridSizeY % 2 == 0 ? gizmoGridSizeY + 1 : gizmoGridSizeY;
            Vector3 scaledCellSize = new Vector3(cellSize.x * transform.lossyScale.x, cellSize.y * transform.lossyScale.y, cellSize.z * transform.lossyScale.z);
            Vector3 scaledCellGap = new Vector3(cellGap.x * transform.lossyScale.x, cellGap.y * transform.lossyScale.y, cellGap.z * transform.lossyScale.z);
            float xn = (scaledCellGap.x + scaledCellSize.x);
            float zn = (scaledCellGap.z + scaledCellSize.z);
            float gridCornerX = (transform.position.x - gridExtentOddX * xn / 2) + xn / 2;
            float gridCornerZ = (transform.position.z - gridExtentOddZ * zn / 2) + zn / 2;

            if (scaledCellGap.x == 0 && scaledCellGap.z == 0)
            {
                // if there are no gaps between cells then we can optimize the drawing of the grid ...
                if (gizmoDrawStyle == 0)
                {
                    gridCornerX -= xn / 2;
                    gridCornerZ -= zn / 2;

                    for (int i = 0; i <= gizmoGridSizeX; ++i)
                    {
                        Vector3 pos = new Vector3(gridCornerX + i * xn, transform.position.y + gizmoGridPositionY, gridCornerZ);

                        Vector3[] verts = new Vector3[2];
                        verts[0] = pos;
                        verts[1] = new Vector3(pos.x, pos.y, pos.z + gizmoGridSizeY * zn);

                        verts[0] -= transform.position;
                        verts[1] -= transform.position;

                        verts[0] = (transform.rotation * verts[0]) + gizmoGridViewBias;
                        verts[1] = (transform.rotation * verts[1]) + gizmoGridViewBias;

                        verts[0] += transform.position;
                        verts[1] += transform.position;

                        Gizmos.DrawLine(verts[0], verts[1]);
                    }

                    for (int j = 0; j <= gizmoGridSizeY; ++j)
                    {
                        Vector3 pos = new Vector3(gridCornerX, transform.position.y + gizmoGridPositionY, gridCornerZ + j * zn);

                        Vector3[] verts = new Vector3[2];
                        verts[0] = pos;
                        verts[1] = new Vector3(pos.x + gizmoGridSizeX * xn, pos.y, pos.z);

                        verts[0] -= transform.position;
                        verts[1] -= transform.position;

                        verts[0] = (transform.rotation * verts[0]) + gizmoGridViewBias;
                        verts[1] = (transform.rotation * verts[1]) + gizmoGridViewBias;

                        verts[0] += transform.position;
                        verts[1] += transform.position;

                        Gizmos.DrawLine(verts[0], verts[1]);
                    }
                }
                else if (gizmoDrawStyle == 1)
                {
                    for (int i = 0; i < gizmoGridSizeX; ++i)
                    {
                        Vector3 pos = new Vector3(gridCornerX + i * xn, transform.position.y + gizmoGridPositionY, gridCornerZ - (zn / 2) + gizmoGridSizeY * zn / 2);
                        Gizmos.matrix = transform.localToWorldMatrix;
                        Gizmos.DrawWireCube(
                            pos - transform.position,
                            new Vector3(scaledCellSize.x, scaledCellSize.y, scaledCellSize.z * gizmoGridSizeY)
                        );
                    }

                    for (int j = 0; j < gizmoGridSizeY; ++j)
                    {
                        Vector3 pos = new Vector3(gridCornerX - (xn / 2) + gizmoGridSizeX * zn / 2, transform.position.y + gizmoGridPositionY, gridCornerZ + j * zn);
                        Gizmos.matrix = transform.localToWorldMatrix;
                        Gizmos.DrawWireCube(
                            pos - transform.position,
                            new Vector3(scaledCellSize.x * gizmoGridSizeX, scaledCellSize.y, scaledCellSize.z)
                        );
                    }
                }
            }
            else
            {
                for (int i = 0; i < gizmoGridSizeX; ++i)
                {
                    for (int j = 0; j < gizmoGridSizeY; ++j)
                    {
                        Vector3 pos = new Vector3(gridCornerX + i * xn, transform.position.y + gizmoGridPositionY, gridCornerZ + j * zn);

                        if (gizmoDrawStyle == 0)
                        {
                            Vector3[] verts = new Vector3[4];
                            verts[0] = new Vector3(pos.x - scaledCellSize.x / 2, pos.y, pos.z - scaledCellSize.z / 2);
                            verts[1] = new Vector3(pos.x - scaledCellSize.x / 2, pos.y, pos.z + scaledCellSize.z / 2);
                            verts[2] = new Vector3(pos.x + scaledCellSize.x / 2, pos.y, pos.z + scaledCellSize.z / 2);
                            verts[3] = new Vector3(pos.x + scaledCellSize.x / 2, pos.y, pos.z - scaledCellSize.z / 2);

                            verts[0] -= transform.position;
                            verts[1] -= transform.position;
                            verts[2] -= transform.position;
                            verts[3] -= transform.position;

                            verts[0] = transform.rotation * verts[0] + gizmoGridViewBias;
                            verts[1] = transform.rotation * verts[1] + gizmoGridViewBias;
                            verts[2] = transform.rotation * verts[2] + gizmoGridViewBias;
                            verts[3] = transform.rotation * verts[3] + gizmoGridViewBias;

                            verts[0] += transform.position;
                            verts[1] += transform.position;
                            verts[2] += transform.position;
                            verts[3] += transform.position;

                            Gizmos.DrawLine(verts[0], verts[1]);
                            Gizmos.DrawLine(verts[1], verts[2]);
                            Gizmos.DrawLine(verts[2], verts[3]);
                            Gizmos.DrawLine(verts[3], verts[0]);
                        }
                        else if (gizmoDrawStyle == 1)
                        {
                            Gizmos.matrix = transform.localToWorldMatrix;
                            Gizmos.DrawWireCube(pos - transform.position, scaledCellSize);
                        }
                    }
                }
            }
        }
#endregion
#endif
    }
}
