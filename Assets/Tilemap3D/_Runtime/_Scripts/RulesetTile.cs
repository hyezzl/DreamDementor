using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using static Tilemap3D.IPaletteTile;

namespace Tilemap3D
{
    [CreateAssetMenu(fileName = "MyRulesetTile", menuName = "Tilemap3D/RulesetTile")]
    public class RulesetTile : ScriptableObject, IPaletteTile, IObservable<RulesetTile>
    {
        public Tile defaultTile;
        public List<Rule> rules = new List<Rule>();

        private void OnValidate()
        {
            foreach (Rule rule in rules)
            {
                if (rule != null && rule.matchGrid != null && rule.matchGrid.Length > 27)
                {
                    rule.matchGrid = new Rule.EMatchType[27] {
                        rule.matchGrid[0], rule.matchGrid[1], rule.matchGrid[2], 
                        rule.matchGrid[3], rule.matchGrid[4], rule.matchGrid[5],
                        rule.matchGrid[6], rule.matchGrid[7], rule.matchGrid[8],
                        rule.matchGrid[9], rule.matchGrid[10], rule.matchGrid[11],
                        rule.matchGrid[12], rule.matchGrid[13], rule.matchGrid[14],
                        rule.matchGrid[15], rule.matchGrid[16], rule.matchGrid[17],
                        rule.matchGrid[18], rule.matchGrid[19], rule.matchGrid[20],
                        rule.matchGrid[21], rule.matchGrid[22], rule.matchGrid[23],
                        rule.matchGrid[24], rule.matchGrid[25], rule.matchGrid[26]
                    };
                }
            }
        }

        /// <returns>
        /// The appropriate tile gameobject based on the current value of <paramref name="placementContext"/>. 
        /// If placement context is null, then this function will return the tile object for slot (0, 0, 0).
        /// </returns>
        public PrefabData GetPrefabData(TileContext placementContext = null)
        {
            Tile tile = defaultTile;

            if (placementContext == null || placementContext.neighbors == null)
                return tile == null ? null : tile.GetPrefabData();

            bool matchFound = false;
            Quaternion ruleRotation = Quaternion.identity;
            Vector3 ruleScale = Vector3.one;
            for (int i = 0; i < rules.Count; i++)
            {
                Rule rule = rules[i];
                Tile ruleTile = rule.tile;
                if (rule.Matches(this, ref placementContext.neighbors))
                {
                    tile = ruleTile;
                    matchFound = true;
                    break;
                }

                switch (rule.transformation)
                {
                    case Rule.ETransformation.Fixed:
                        break;

                    case Rule.ETransformation.RotateY:
                        if (rule.MatchesRotation(this, ref placementContext.neighbors, ref ruleRotation, Rule.ERuleRotationAxis.Y))
                        {
                            tile = ruleTile;
                            matchFound = true;
                        }
                        break;

                    case Rule.ETransformation.MirrorX:
                        if (rule.MatchesMirrored(this, ref placementContext.neighbors, ref ruleScale, Rule.ERuleMirrorAxis.X))
                        {
                            tile = ruleTile;
                            matchFound = true;
                        }
                        break;

                    case Rule.ETransformation.MirrorY:
                        if (rule.MatchesMirrored(this, ref placementContext.neighbors, ref ruleScale, Rule.ERuleMirrorAxis.Y))
                        {
                            tile = ruleTile;
                            matchFound = true;
                        }
                        break;

                    case Rule.ETransformation.MirrorZ:
                        if (rule.MatchesMirrored(this, ref placementContext.neighbors, ref ruleScale, Rule.ERuleMirrorAxis.Z))
                        {
                            tile = ruleTile;
                            matchFound = true;
                        }
                        break;

                    case Rule.ETransformation.MirrorXZ:
                        if (rule.MatchesMirrored(this, ref placementContext.neighbors, ref ruleScale, Rule.ERuleMirrorAxis.XZ))
                        {
                            tile = ruleTile;
                            matchFound = true;
                        }
                        break;

                    case Rule.ETransformation.MirrorXY:
                        if (rule.MatchesMirrored(this, ref placementContext.neighbors, ref ruleScale, Rule.ERuleMirrorAxis.XY))
                        {
                            tile = ruleTile;
                            matchFound = true;
                        }
                        break;

                    case Rule.ETransformation.MirrorYZ:
                        if (rule.MatchesMirrored(this, ref placementContext.neighbors, ref ruleScale, Rule.ERuleMirrorAxis.YZ))
                        {
                            tile = ruleTile;
                            matchFound = true;
                        }
                        break;
                    
                    default:
                        break;
                }

                if (matchFound)
                    break;
            }

            return new PrefabData()
            {
                prefab = tile == null ? null : tile.gameObject,
                ruleRotation = ruleRotation,
                ruleScale = ruleScale
            };
        }

        public void ValidateRulesetTilesInScene()
        {
            rulesAreDirty = true;
            NotifyAllSubscribers();
            rulesAreDirty = false;
        }

        private void OnDestroy()
        {
            DetachAllSubscribers();
        }

        #region Observer Pattern
        protected List<IObserver<RulesetTile>> observers = new List<IObserver<RulesetTile>>();
        [HideInInspector] public bool rulesAreDirty;

        public IDisposable Subscribe(IObserver<RulesetTile> observer)
        {
            observers?.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<RulesetTile>> observers;
            private readonly IObserver<RulesetTile> observer;

            public Unsubscriber(List<IObserver<RulesetTile>> observers, IObserver<RulesetTile> observer)
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
