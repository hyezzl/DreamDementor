using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using static Tilemap3D.IPaletteTile;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tilemap3D
{
    [ExecuteInEditMode]
    [AddComponentMenu("Tilemap3D/Tile")]
    public class Tile : MonoBehaviour, IPaletteTile
    {
        [SerializeField, HideInInspector] private GameObject sourcePrefab;
        [SerializeField, HideInInspector] private Vector3 placementOffset;
        [SerializeField, HideInInspector] private Vector3 rotationOffset;
        [SerializeField, HideInInspector] private Vector3 scaleOffset = Vector3.one;

        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TileLayer layer;
        [SerializeField] private Vector3Int gridCellPosition;

        /// <returns>
        /// The gameobject this tile is attached to. Note that <paramref name="placementContext"/> is ignored and discarded.
        /// </returns>
        public PrefabData GetPrefabData(TileContext placementContext = null)
        {
            return new PrefabData() { prefab = gameObject };
        }

        private void Awake()
        {
#if UNITY_EDITOR
            if (SourcePrefab == null && PrefabUtility.IsPartOfAnyPrefab(gameObject))
            {
                SourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            }
#endif
        }

        private void OnDestroy()
        {
            if (Tilemap != null)
                Tilemap.Remove(new TileKey(this), false);
        }

        public void AddToTilemap(Tilemap tilemap, Vector3Int gridCellPosition, TileLayer tileLayer)
        {
            if (Tilemap != null)
                Tilemap.Remove(new TileKey(this));

            tilemap.SetTile(gridCellPosition, tileLayer, this);
            Tilemap = tilemap;
            Layer = tileLayer;
            GridCellPosition = gridCellPosition;
        }

        public Tilemap Tilemap
        {
            get => tilemap;
            internal set => tilemap = value;
        }

        public TileLayer Layer 
        {
            get => layer;
            internal set => layer = value;
        }

        public Vector3Int GridCellPosition 
        { 
            get => gridCellPosition;
            internal set => gridCellPosition = value; 
        }

        public GameObject SourcePrefab
        {
            get => sourcePrefab;
            private set => sourcePrefab = value;
        }

        public Vector3 PlacementOffset { get => placementOffset; set => placementOffset = value; }
        public Vector3 RotationOffset { get => rotationOffset; set => rotationOffset = value; }
        public Vector3 ScaleOffset { get => scaleOffset; set => scaleOffset = value; }

        public static Tile[] GetNeighborTiles(Tilemap tilemap, TileLayer tileLayer, Vector3Int gridCellPosition)
        {
            Tile[] neighborTiles = new Tile[27];

            neighborTiles[4] = tilemap.GetTile(gridCellPosition + Vector3Int.down, tileLayer);
            neighborTiles[1] = tilemap.GetTile(gridCellPosition + Vector3Int.down + Vector3Int.forward, tileLayer);
            neighborTiles[7] = tilemap.GetTile(gridCellPosition + Vector3Int.down + Vector3Int.back, tileLayer);
            neighborTiles[5] = tilemap.GetTile(gridCellPosition + Vector3Int.down + Vector3Int.right, tileLayer);
            neighborTiles[3] = tilemap.GetTile(gridCellPosition + Vector3Int.down + Vector3Int.left, tileLayer);
            neighborTiles[2] = tilemap.GetTile(gridCellPosition + Vector3Int.down + Vector3Int.right + Vector3Int.forward, tileLayer);
            neighborTiles[0] = tilemap.GetTile(gridCellPosition + Vector3Int.down + Vector3Int.left + Vector3Int.forward, tileLayer);
            neighborTiles[8] = tilemap.GetTile(gridCellPosition + Vector3Int.down + Vector3Int.right + Vector3Int.back, tileLayer);
            neighborTiles[6] = tilemap.GetTile(gridCellPosition + Vector3Int.down + Vector3Int.left + Vector3Int.back, tileLayer);

            neighborTiles[10] = tilemap.GetTile(gridCellPosition + Vector3Int.forward, tileLayer);
            neighborTiles[16] = tilemap.GetTile(gridCellPosition + Vector3Int.back, tileLayer);
            neighborTiles[14] = tilemap.GetTile(gridCellPosition + Vector3Int.right, tileLayer);
            neighborTiles[12] = tilemap.GetTile(gridCellPosition + Vector3Int.left, tileLayer);
            neighborTiles[11] = tilemap.GetTile(gridCellPosition + Vector3Int.right + Vector3Int.forward, tileLayer);
            neighborTiles[9] = tilemap.GetTile(gridCellPosition + Vector3Int.left + Vector3Int.forward, tileLayer);
            neighborTiles[17] = tilemap.GetTile(gridCellPosition + Vector3Int.right + Vector3Int.back, tileLayer);
            neighborTiles[15] = tilemap.GetTile(gridCellPosition + Vector3Int.left + Vector3Int.back, tileLayer);

            neighborTiles[22] = tilemap.GetTile(gridCellPosition + Vector3Int.up, tileLayer);
            neighborTiles[19] = tilemap.GetTile(gridCellPosition + Vector3Int.up + Vector3Int.forward, tileLayer);
            neighborTiles[25] = tilemap.GetTile(gridCellPosition + Vector3Int.up + Vector3Int.back, tileLayer);
            neighborTiles[23] = tilemap.GetTile(gridCellPosition + Vector3Int.up + Vector3Int.right, tileLayer);
            neighborTiles[21] = tilemap.GetTile(gridCellPosition + Vector3Int.up + Vector3Int.left, tileLayer);
            neighborTiles[20] = tilemap.GetTile(gridCellPosition + Vector3Int.up + Vector3Int.right + Vector3Int.forward, tileLayer);
            neighborTiles[18] = tilemap.GetTile(gridCellPosition + Vector3Int.up + Vector3Int.left + Vector3Int.forward, tileLayer);
            neighborTiles[26] = tilemap.GetTile(gridCellPosition + Vector3Int.up + Vector3Int.right + Vector3Int.back, tileLayer);
            neighborTiles[24] = tilemap.GetTile(gridCellPosition + Vector3Int.up + Vector3Int.left + Vector3Int.back, tileLayer);

            return neighborTiles;
        }
    }
}
