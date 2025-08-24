using UnityEngine;

using System;
using System.Collections.Generic;

using Tilemap3D.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tilemap3D
{
    /// <summary>
    /// A Monobehaviour used to keep track of ruleset tiles in the tilemap system.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    [RequireComponent(typeof(Tile))]
    public class RulesetTileBehavior : MonoBehaviour, IObserver<RulesetTile>
    {
        [SerializeField, HideInInspector] private RulesetTile rulesetTile;
        [SerializeField, HideInInspector] private GameObject sourcePrefab;

        [SerializeField, HideInInspector] public Quaternion ruleRotation;
        [SerializeField, HideInInspector] public Vector3 ruleScale = Vector3.one;

        private Tile tile;

        public RulesetTile RulesetTile
        {
            get => rulesetTile;
            set
            {
                bool same = rulesetTile == value;

                rulesetTile = value;

                if (unsubscriber == null || !same)
                    Subscribe(rulesetTile);
            }
        }

        public Tile Tile
        {
            get 
            {
                if (tile == null)
                    tile = GetComponent<Tile>();

                return tile;
            }
        }

        public GameObject SourcePrefab
        {
            get => sourcePrefab;
            set => sourcePrefab = value;
        }

        private void Awake()
        {
            RulesetTile = rulesetTile;

#if UNITY_EDITOR
            if (SourcePrefab == null && PrefabUtility.IsPartOfAnyPrefab(gameObject))
            {
                SourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            }
#endif
        }

        private void OnEnable()
        {
            RulesetTile = rulesetTile;
        }

        private void OnDestroy()
        {
            unsubscriber?.Dispose();
            unsubscriber = null;
        }

        /// <summary>
        /// Evaluates this ruleset tile's neighbors that are of the same RulesetTile. <br />
        /// If cascadeCheck is true, then neighbors will only be evaluated if this tile is replaced during evaluation.
        /// </summary>
        /// <param name="cascadeCheck">[default = false] whether or not to cascade the evaluation to neighbors (uses BFS algorithm).</param>
        public void EvaluateNeighborRules(bool cascadeCheck = false)
        {
#if UNITY_EDITOR
            Tilemap tilemap = GetComponentInParent<Tilemap>();
            TileLayer tileLayer = GetComponentInParent<TileLayer>();

            if (Tile == null || tileLayer == null || tilemap == null)
                return;

            Vector3Int currentCell = Tile.GridCellPosition;
            Tile[] neighborTiles = Tile.GetNeighborTiles(tilemap, tileLayer, currentCell);
            GameObject replacement = EvaluateRulesetTile(tilemap, neighborTiles, this);

            if (replacement != null)
            {
                Tile prevTile = tilemap.GetTile(currentCell, tileLayer);
                int prevChildIndex = prevTile == null ? -1 : prevTile.transform.GetSiblingIndex();

                replacement.GetComponent<Tile>().AddToTilemap(tilemap, currentCell, tileLayer);

                if (prevChildIndex >= 0)
                    replacement.transform.SetSiblingIndex(prevChildIndex);
            }

            if (cascadeCheck)
                BFSEvaluateNeighborRules(tilemap, tileLayer, currentCell);
#endif
        }

        private void BFSEvaluateNeighborRules(Tilemap tilemap, TileLayer tileLayer, Vector3Int startCell)
        {
#if UNITY_EDITOR
            if (tilemap == null || tileLayer == null)
                return;

            PriorityQueue<int, Vector3Int> openlist = new PriorityQueue<int, Vector3Int>();
            openlist.Push(0, startCell);
            HashSet<Vector3Int> closedlist = new HashSet<Vector3Int> { startCell };

            while (!openlist.Empty)
            {
                Vector3Int currentCell = openlist.Pop().Value;

                Tile tile = tilemap.GetTile(currentCell, tileLayer);
                if (tile == null)
                    continue;

                RulesetTileBehavior rulesetTileBehavior = tile.GetComponent<RulesetTileBehavior>();

                Tile[] neighborTiles = Tile.GetNeighborTiles(tilemap, tileLayer, currentCell);

                bool changed = tile.GridCellPosition == startCell;

                // evaluate the current cell, if tile needs to be replaced then continue traversal
                if (tile.GridCellPosition != startCell)
                {
                    GameObject replacement = EvaluateRulesetTile(tilemap, neighborTiles, rulesetTileBehavior);

                    if (replacement != null)
                    {
                        int prevChildIndex = rulesetTileBehavior.transform.GetSiblingIndex();
                        replacement.GetComponent<Tile>().AddToTilemap(tilemap, currentCell, tileLayer);
                        replacement.transform.SetSiblingIndex(prevChildIndex);
                        changed = true;
                    }
                }

                if (!changed)
                    continue;

                foreach (Tile nextTile in neighborTiles)
                {
                    if (nextTile == null) 
                        continue;

                    if (!closedlist.Contains(nextTile.GridCellPosition))
                    {
                        openlist.Push(0, nextTile.GridCellPosition);
                        closedlist.Add(nextTile.GridCellPosition);
                    }
                }
            }
#endif
        }

        private GameObject EvaluateRulesetTile(Tilemap tilemap, Tile[] neighborTiles, RulesetTileBehavior rulesetTileBehavior)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return null;

            if (neighborTiles == null || neighborTiles.Length == 0)
                return null;

            if (rulesetTileBehavior == null || rulesetTileBehavior.rulesetTile != rulesetTile)
                return null;

            TileContext tileContext = new TileContext()
            {
                neighbors = new Tile[27]
            };

            for (int i = 0; i < neighborTiles.Length; i++)
                tileContext.SetNeighborTile(i, neighborTiles[i]);

            IPaletteTile.PrefabData prefabData = rulesetTileBehavior.rulesetTile.GetPrefabData(tileContext);
            GameObject prefab = prefabData == null ? null : prefabData.prefab;

            if (prefab == null)
            {
                string rtName = rulesetTileBehavior.RulesetTile == null ? "" : rulesetTileBehavior.RulesetTile.name;
                Debug.LogWarning($"A matched rule from the ruleset tile '{rtName}' has it's tile property set to null or maybe the prefab asset is missing?");
                return null;
            }

            if (prefab != rulesetTileBehavior.SourcePrefab)
            {
                // replace tile (only doable in the editor) ...
                
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, rulesetTileBehavior.transform.parent);
                if (!PrefabUtility.IsPartOfPrefabInstance(rulesetTileBehavior.gameObject))
                    PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                instance.transform.position = tilemap.ConvertToVector3Position(rulesetTileBehavior.Tile.GridCellPosition) + rulesetTileBehavior.Tile.PlacementOffset;

                Quaternion rotation = prefabData.ruleRotation * Quaternion.Euler(rulesetTileBehavior.Tile.RotationOffset);
                instance.transform.localRotation = Quaternion.Euler(rotation.eulerAngles) * instance.transform.localRotation;

                Vector3 localScaleMultipliers = rulesetTileBehavior.Tile.ScaleOffset;
                localScaleMultipliers = new Vector3(
                    localScaleMultipliers.x * prefabData.ruleScale.x,
                    localScaleMultipliers.y * prefabData.ruleScale.y,
                    localScaleMultipliers.z * prefabData.ruleScale.z
                );
                Vector3 localScale = instance.transform.localScale;
                instance.transform.localScale = new Vector3(
                    localScale.x * localScaleMultipliers.x,
                    localScale.y * localScaleMultipliers.y,
                    localScale.z * localScaleMultipliers.z
                );

                RulesetTileBehavior newRulesetTileBehavior = instance.AddComponent<RulesetTileBehavior>();
                newRulesetTileBehavior.RulesetTile = rulesetTileBehavior.RulesetTile;
                newRulesetTileBehavior.SourcePrefab = prefab;
                newRulesetTileBehavior.Tile.PlacementOffset = rulesetTileBehavior.Tile.PlacementOffset;
                newRulesetTileBehavior.Tile.RotationOffset = rulesetTileBehavior.Tile.RotationOffset;
                newRulesetTileBehavior.Tile.ScaleOffset = rulesetTileBehavior.Tile.ScaleOffset;
                newRulesetTileBehavior.ruleRotation = prefabData.ruleRotation;
                newRulesetTileBehavior.ruleScale = prefabData.ruleScale;

                return instance;
            }
#endif
            return null;
        }

        #region Observer Pattern
        private IDisposable unsubscriber;

        private void Subscribe(RulesetTile subject)
        {
            Unsubscribe();

            if (subject == null)
                return;

            unsubscriber = subject.Subscribe(this);
        }

        private void Unsubscribe()
        {
            unsubscriber?.Dispose();
            unsubscriber = null;
        }

        /// <summary>Notifies this observer that the subject has finished sending push-based notifications.</summary>
        public void OnCompleted()
        {
            unsubscriber?.Dispose();
        }

        /// <summary>Notifies this observer that the subject has experienced an error condition.</summary>
        public void OnError(Exception error)
        {
            // do nothing
        }

        /// <summary>Notifies this observer that the subject's state has changed.</summary>
        public void OnNext(RulesetTile value)
        {
#if UNITY_EDITOR
            if (value.rulesAreDirty)
                EvaluateNeighborRules(false);
#endif
        }
        #endregion
    }
}
