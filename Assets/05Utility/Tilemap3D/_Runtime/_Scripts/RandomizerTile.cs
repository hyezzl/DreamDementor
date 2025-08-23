using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using static Tilemap3D.IPaletteTile;

namespace Tilemap3D
{
    [CreateAssetMenu(fileName = "MyRandomizerTile", menuName = "Tilemap3D/RandomizerTile")]
    public class RandomizerTile : ScriptableObject, IPaletteTile, IObservable<RandomizerTile>
    {
        public List<Tile> tiles = new List<Tile>();

        private const string WARNING_RANDOMIZER_PALETTE_TILE = "Warning: Detected invalid tile in randomizer list, removing it. " +
                                                               "You can only add Tile prefabs that are not null to a randomizer list.";

        private int seed;

        private void OnValidate()
        {
            while (tiles != null && tiles.Count > 0)
            {
                Tile tile = tiles[tiles.Count - 1];
                GameObject tilePrefab = tile == null ? null : tile.gameObject;
                
                if (tilePrefab != null && tilePrefab.scene.name != null)
                {
                    tiles.RemoveAt(tiles.Count - 1);
                    Debug.LogWarning(WARNING_RANDOMIZER_PALETTE_TILE, this);
                }
                else
                    break;
            }
        }

        public PrefabData GetPrefabData(TileContext placementContext = null)
        {
            if (seed == 0)
            {
                seed = (int)DateTimeOffset.Now.ToUnixTimeMilliseconds();
                UnityEngine.Random.InitState(seed);
            }

            GameObject prefab = null;
            if (tiles.Count > 0)
                prefab = tiles[UnityEngine.Random.Range(0, tiles.Count)].gameObject;

            return new PrefabData() { prefab = prefab };
        }

        public void ReRandomizeTilesInScene()
        {
            tileListHasChanged = true;
            NotifyAllSubscribers();
            tileListHasChanged = false;
        }

        private void OnDestroy()
        {
            DetachAllSubscribers();
        }

        #region Observer Pattern
        protected List<IObserver<RandomizerTile>> observers = new List<IObserver<RandomizerTile>>();
        [HideInInspector] public bool tileListHasChanged;

        public IDisposable Subscribe(IObserver<RandomizerTile> observer)
        {
            observers?.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<RandomizerTile>> observers;
            private readonly IObserver<RandomizerTile> observer;

            public Unsubscriber(List<IObserver<RandomizerTile>> observers, IObserver<RandomizerTile> observer)
            {
                this.observers = observers;
                this.observer = observer;
            }

            public void Dispose()
            {
                if (observer != null && observers.Contains(observer))
                    observers.Remove(observer);
            }
        }

        protected void NotifyAllSubscribers()
        {
            foreach (var observer in observers.ToArray())
                observer.OnNext(this);
        }

        protected void DetachAllSubscribers()
        {
            if (observers == null) return;

            foreach (var observer in observers.ToArray())
                observer.OnCompleted();

            observers.Clear();
        }
        #endregion
    }
}
