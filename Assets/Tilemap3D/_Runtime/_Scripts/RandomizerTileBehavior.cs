using UnityEngine;

using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tilemap3D
{
    /// <summary>
    /// A Monobehaviour used to keep track of randomizer tiles in the tilemap system.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    [RequireComponent(typeof(Tile))]
    public class RandomizerTileBehavior : MonoBehaviour, IObserver<RandomizerTile>
    {
        [SerializeField, HideInInspector] private RandomizerTile randomizerTile;

        private Tile tile;

        public RandomizerTile RandomizerTile
        {
            get => randomizerTile;
            set
            {
                bool same = randomizerTile == value;

                randomizerTile = value;

                if (unsubscriber == null || !same)
                    Subscribe(randomizerTile);
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

        private void Awake()
        {
            RandomizerTile = randomizerTile;
        }

        private void OnEnable()
        {
            RandomizerTile = randomizerTile;
        }

        private void OnDestroy()
        {
            unsubscriber?.Dispose();
            unsubscriber = null;
        }

        public void ReRandomize()
        {
#if UNITY_EDITOR
            if (RandomizerTile == null || Tile == null)
                return;

            Tilemap tilemap = Tile.Tilemap;
            TileLayer tileLayer = Tile.Layer;

            if (tileLayer == null || tilemap == null)
                return;

            Vector3Int currentCell = Tile.GridCellPosition;
            IPaletteTile.PrefabData prefabData = RandomizerTile.GetPrefabData();
            GameObject prefab = prefabData == null ? null : prefabData.prefab;
            GameObject instance = null;

            int prevChildIndex = transform.GetSiblingIndex();

            if (prefab != null)
            {
                instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform.parent);
                if (!PrefabUtility.IsPartOfPrefabInstance(gameObject))
                    PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }

            Tile replacementTile = instance == null ? null : instance.GetComponent<Tile>();
            if (replacementTile != null)
            {
                instance.transform.position = tilemap.ConvertToVector3Position(Tile.GridCellPosition) + Tile.PlacementOffset;

                Quaternion rotation = Quaternion.Euler(Tile.RotationOffset);
                instance.transform.localRotation = Quaternion.Euler(rotation.eulerAngles) * instance.transform.localRotation;

                Vector3 localScaleMultipliers = Tile.ScaleOffset;
                Vector3 localScale = instance.transform.localScale;
                instance.transform.localScale = new Vector3(
                    localScale.x * localScaleMultipliers.x,
                    localScale.y * localScaleMultipliers.y,
                    localScale.z * localScaleMultipliers.z
                );

                RandomizerTileBehavior newRandomizerTileBehavior = replacementTile.gameObject.AddComponent<RandomizerTileBehavior>();
                newRandomizerTileBehavior.RandomizerTile = RandomizerTile;
                replacementTile.PlacementOffset = Tile.PlacementOffset;
                replacementTile.RotationOffset = Tile.RotationOffset;
                replacementTile.ScaleOffset = Tile.ScaleOffset;

                tilemap.SetTile(currentCell, tileLayer, replacementTile);

                if (prevChildIndex >= 0)
                    replacementTile.transform.SetSiblingIndex(prevChildIndex);
            }
#endif
        }

        #region Observer Pattern
        private IDisposable unsubscriber;

        private void Subscribe(RandomizerTile subject)
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
        public void OnNext(RandomizerTile value)
        {
#if UNITY_EDITOR
            if (value.tileListHasChanged)
                ReRandomize();
#endif
        }
        #endregion
    }
}
