using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using Tilemap3D;

using UnityObject = UnityEngine.Object;

namespace Tilemap3DEditor
{
    public static class TilemapEditorUtility
    {
        public static void AddTile(Tilemap tilemap, TileLayer tileLayer, Vector3Int gridCellPosition, UnityObject paletteTileObject, 
            bool unpackPrefab, Vector3 placementOffset, Vector3 rotationOffset, Vector3 scaleOffset)
        {
            if (tilemap == null || tileLayer == null || paletteTileObject == null || 
                !(PrefabUtility.IsPartOfAnyPrefab(paletteTileObject) || paletteTileObject is ScriptableObject))
                return;
            
            if (tileLayer.GetComponentInParent<Tilemap>() != tilemap)
                return;

            IPaletteTile paletteTile = paletteTileObject as IPaletteTile;
            RulesetTile rulesetTile = paletteTile as RulesetTile;
            RandomizerTile randomizerTile = paletteTile as RandomizerTile;

            TileContext placementContext = new TileContext()
            {
                neighbors = Tile.GetNeighborTiles(tilemap, tileLayer, gridCellPosition)
            };

            IPaletteTile.PrefabData prefabData = paletteTile.GetPrefabData(placementContext);
            GameObject tilePrefab = prefabData == null ? null : prefabData.prefab;
            if (tilePrefab == null)
                return;

            Tile prevTile = tilemap.GetTile(gridCellPosition, tileLayer);
            int prevChildIndex = prevTile == null ? -1 : prevTile.transform.GetSiblingIndex();

            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab, tileLayer.transform);
            if (unpackPrefab && PrefabUtility.IsPartOfAnyPrefab(prefabInstance))
                PrefabUtility.UnpackPrefabInstance(prefabInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            if (prefabInstance == null)
                return;

            RandomizerTileBehavior newRandomizerTileBehavior = null;
            if (randomizerTile != null)
            {
                newRandomizerTileBehavior = prefabInstance.AddComponent<RandomizerTileBehavior>();
                newRandomizerTileBehavior.RandomizerTile = randomizerTile;
            }

            RulesetTileBehavior newRulesetBehavior = null;
            if (rulesetTile != null)
            {
                newRulesetBehavior = prefabInstance.AddComponent<RulesetTileBehavior>();
                newRulesetBehavior.RulesetTile = rulesetTile;
                newRulesetBehavior.ruleRotation = prefabData.ruleRotation;
                newRulesetBehavior.ruleScale = prefabData.ruleScale;
            }

            Tile newTile = prefabInstance.GetComponent<Tile>();

            newTile.PlacementOffset = placementOffset;
            newTile.RotationOffset = rotationOffset;
            newTile.ScaleOffset = scaleOffset;

            newTile.AddToTilemap(tilemap, gridCellPosition, tileLayer);

            Tile replacedTile = null;
            if (newRulesetBehavior != null)
            {
                newRulesetBehavior.EvaluateNeighborRules(true);
                replacedTile = tilemap.GetTile(gridCellPosition, tileLayer);
            }

            bool tileWasReplaced = replacedTile != null && newTile != replacedTile;
            if (tileWasReplaced)
            {
                // after ruleset tile neighbor evaluation the tile instance might have been replaced,
                // so we must re-assign references
                newTile = replacedTile;
                newRulesetBehavior = newTile.GetComponent<RulesetTileBehavior>();
            }

            foreach (Tile neighborTile in placementContext.neighbors)
            {
                if (neighborTile != null && neighborTile.TryGetComponent(out RulesetTileBehavior rtb))
                    rtb.EvaluateNeighborRules(true);
            }

            // adjust instance with placement options ...
            if (unpackPrefab && PrefabUtility.IsPartOfAnyPrefab(newTile))
                PrefabUtility.UnpackPrefabInstance(newTile.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            if (prevChildIndex >= 0)
                newTile.transform.SetSiblingIndex(prevChildIndex);

            EditorUtility.SetDirty(tilemap.gameObject);

            if (tileWasReplaced)
                return;

            newTile.transform.position = tilemap.ConvertToVector3Position(gridCellPosition) + tilemap.transform.rotation * placementOffset;

            Quaternion rotation = prefabData.ruleRotation * Quaternion.Euler(rotationOffset);
            newTile.transform.localRotation = Quaternion.Euler(rotation.eulerAngles) * newTile.transform.localRotation;

            Vector3 localScaleMultipliers = new Vector3(
                scaleOffset.x * prefabData.ruleScale.x,
                scaleOffset.y * prefabData.ruleScale.y,
                scaleOffset.z * prefabData.ruleScale.z
            );

            newTile.transform.localScale = new Vector3(
                newTile.transform.localScale.x * localScaleMultipliers.x,
                newTile.transform.localScale.y * localScaleMultipliers.y,
                newTile.transform.localScale.z * localScaleMultipliers.z
            );
        }

        public static void RemoveTile(Tilemap tilemap, TileLayer tileLayer, Vector3Int gridCellPosition)
        {
            RemoveTiles(tilemap, tileLayer, gridCellPosition, Vector3Int.one);
        }
        public static void RemoveTiles(Tilemap tilemap, TileLayer tileLayer, Vector3Int gridCellPosition, Vector3Int removalAreaSize)
        {
            if (tilemap == null || tileLayer == null)
                return;

            tilemap.Remove(new TileKey(gridCellPosition, tileLayer), true);

            removalAreaSize.x = removalAreaSize.x < 1 ? 1 : removalAreaSize.x;
            removalAreaSize.y = removalAreaSize.y < 1 ? 1 : removalAreaSize.y;
            removalAreaSize.z = removalAreaSize.z < 1 ? 1 : removalAreaSize.z;

            Vector3Int eraserCornerCell = Vector3Int.one;
            eraserCornerCell.x = gridCellPosition.x - removalAreaSize.x / 2;
            eraserCornerCell.y = gridCellPosition.y - removalAreaSize.y / 2;
            eraserCornerCell.z = gridCellPosition.z - removalAreaSize.z / 2;

            List<Vector3Int> borderCells = new List<Vector3Int>();

            for (int x = 0; x < removalAreaSize.x; x++)
                for (int y = 0; y < removalAreaSize.y; y++)
                    for (int z = 0; z < removalAreaSize.z; z++)
                        EraseCellAndCollectBorderCells(x, y, z);

            void EraseCellAndCollectBorderCells(int x, int y, int z)
            {
                Vector3Int cellToErase = new Vector3Int(eraserCornerCell.x + x, eraserCornerCell.y + y, eraserCornerCell.z + z);
                tilemap.Remove(cellToErase, tileLayer);

                borderCells.AddRange(GetBorderCellsHelper(cellToErase, x, y, z, removalAreaSize));
            };

            foreach (Vector3Int borderCell in borderCells)
                EvaluateTile(tilemap.GetTile(borderCell, tileLayer));

            EditorSceneManager.MarkSceneDirty(tilemap.gameObject.scene);
        }

        private static void EvaluateTile(Tile tile)
        {
            if (tile != null && tile.TryGetComponent(out RulesetTileBehavior rulesetTileBehavior))
                rulesetTileBehavior.EvaluateNeighborRules(true);
        }

        public static void ReplaceTile(Tile tile, Tile replacementTile, bool evaluateRulesetTileNeighborsOnChange = true)
        {
            if (tile == null || replacementTile == null || tile.gameObject.scene == null)
                return;

            Tilemap tilemap = tile.Tilemap;
            TileLayer tileLayer = tile.Layer;

            if (tilemap == null || tileLayer == null)
                return;

            RulesetTileBehavior rulesetTileBehavior = tile.GetComponent<RulesetTileBehavior>();
            RulesetTileBehavior replacementRulesetTileBehavior = replacementTile.GetComponent<RulesetTileBehavior>();
            TileContext tileContext = null;

            if (replacementRulesetTileBehavior != null)
            {
                // we are replacing some Tile with a ruleset Tile ...
                tileContext.neighbors = Tile.GetNeighborTiles(tilemap, tileLayer, tile.GridCellPosition);

                IPaletteTile.PrefabData prefabData = replacementRulesetTileBehavior.RulesetTile.GetPrefabData(tileContext);
                GameObject prefab = prefabData == null ? null : prefabData.prefab;

                if (prefab == null)
                {
                    string rtName = replacementRulesetTileBehavior.RulesetTile == null ? "" : replacementRulesetTileBehavior.RulesetTile.name;
                    Debug.LogWarning($"A matched rule from the ruleset tile '{rtName}' has it's tile property set to null or maybe the prefab asset is missing?");
                    return;
                }

                if (rulesetTileBehavior.SourcePrefab != prefab)
                {
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, tile.transform.parent);
                    if (!PrefabUtility.IsPartOfPrefabInstance(rulesetTileBehavior.gameObject))
                        PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                    instance.transform.position = tilemap.ConvertToVector3Position(tile.GridCellPosition) + tilemap.transform.rotation * tile.PlacementOffset;

                    Quaternion rotation = prefabData.ruleRotation * Quaternion.Euler(tile.RotationOffset);
                    instance.transform.localRotation = Quaternion.Euler(rotation.eulerAngles) * instance.transform.localRotation;

                    Vector3 localScaleMultipliers = tile.ScaleOffset;
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
                    newRulesetTileBehavior.Tile.PlacementOffset = tile.PlacementOffset;
                    newRulesetTileBehavior.Tile.RotationOffset = tile.RotationOffset;
                    newRulesetTileBehavior.Tile.ScaleOffset = tile.ScaleOffset;
                    newRulesetTileBehavior.ruleRotation = prefabData.ruleRotation;
                    newRulesetTileBehavior.ruleScale = prefabData.ruleScale;

                    if (evaluateRulesetTileNeighborsOnChange)
                        newRulesetTileBehavior.EvaluateNeighborRules(true);
                }
            }
            else
            {
                // we are replacing some Tile with a non-ruleset Tile ...
                Tile newReplacementTile;
                if (PrefabUtility.IsPartOfPrefabAsset(replacementTile))
                {
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(replacementTile.GetPrefabData(tileContext).prefab, tile.transform.parent);
                    newReplacementTile = instance.GetComponent<Tile>();
                }
                else
                    newReplacementTile = replacementTile;

                if (newReplacementTile == null)
                    return;

                tilemap.SetTile(tile.GridCellPosition, tileLayer, newReplacementTile);
            }
        }

        public static void AdjustTilePositions(Tilemap tilemap, bool registerObjectUndo = true)
        {
            if (tilemap == null)
                return;

            foreach (KeyValuePair<TileKey, Tile> kv in tilemap)
            {
                Tile tile = kv.Value;
                if (tile == null)
                    continue;

                if (registerObjectUndo)
                    Undo.RecordObject(tile.transform, "Adjust Tile Position");

                tile.transform.position = tilemap.ConvertToVector3Position(tile.GridCellPosition) + tilemap.transform.rotation * tile.PlacementOffset;
            }

            if (registerObjectUndo)
                Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        #region Helper Methods
        private static List<Vector3Int> GetBorderCellsHelper(Vector3Int targetCell, int x, int y, int z, Vector3Int areaSize)
        {
            List<Vector3Int> borderCells = new List<Vector3Int>();

            if (x != 0 && x != areaSize.x - 1 && y != 0 && y != areaSize.y - 1 && z != 0 && z != areaSize.z - 1)
                return borderCells;

            if (x == 0)
                borderCells.Add(new Vector3Int(targetCell.x - 1, targetCell.y, targetCell.z));

            if (x == areaSize.x - 1)
                borderCells.Add(new Vector3Int(targetCell.x + 1, targetCell.y, targetCell.z));

            if (y == 0)
                borderCells.Add(new Vector3Int(targetCell.x, targetCell.y - 1, targetCell.z));

            if (y == areaSize.y - 1)
                borderCells.Add(new Vector3Int(targetCell.x, targetCell.y + 1, targetCell.z));

            if (z == 0)
                borderCells.Add(new Vector3Int(targetCell.x, targetCell.y, targetCell.z - 1));

            if (z == areaSize.z - 1)
                borderCells.Add(new Vector3Int(targetCell.x, targetCell.y, targetCell.z + 1));

            if (x == 0 && y == 0)
                borderCells.Add(new Vector3Int(targetCell.x - 1, targetCell.y - 1, targetCell.z));

            if (x == 0 && y == areaSize.y - 1)
                borderCells.Add(new Vector3Int(targetCell.x - 1, targetCell.y + 1, targetCell.z));

            if (x == areaSize.x - 1 && y == areaSize.y - 1)
                borderCells.Add(new Vector3Int(targetCell.x + 1, targetCell.y + 1, targetCell.z));

            if (x == areaSize.x - 1 && y == 0)
                borderCells.Add(new Vector3Int(targetCell.x + 1, targetCell.y - 1, targetCell.z));

            if (x == 0 && z == 0)
                borderCells.Add(new Vector3Int(targetCell.x - 1, targetCell.y, targetCell.z - 1));

            if (x == 0 && z == areaSize.z - 1)
                borderCells.Add(new Vector3Int(targetCell.x - 1, targetCell.y, targetCell.z + 1));

            if (x == areaSize.x - 1 && z == areaSize.z - 1)
                borderCells.Add(new Vector3Int(targetCell.x + 1, targetCell.y, targetCell.z + 1));

            if (x == areaSize.x - 1 && z == 0)
                borderCells.Add(new Vector3Int(targetCell.x + 1, targetCell.y, targetCell.z - 1));

            if (y == 0 && z == 0)
                borderCells.Add(new Vector3Int(targetCell.x, targetCell.y - 1, targetCell.z - 1));

            if (y == 0 && z == areaSize.z - 1)
                borderCells.Add(new Vector3Int(targetCell.x, targetCell.y - 1, targetCell.z + 1));

            if (y == areaSize.y - 1 && z == areaSize.z - 1)
                borderCells.Add(new Vector3Int(targetCell.x, targetCell.y + 1, targetCell.z + 1));

            if (y == areaSize.y - 1 && z == 0)
                borderCells.Add(new Vector3Int(targetCell.x, targetCell.y + 1, targetCell.z - 1));

            if (x == 0 && y == 0 && z == 0)
                borderCells.Add(new Vector3Int(targetCell.x - 1, targetCell.y - 1, targetCell.z - 1));

            if (x == 0 && y == 0 && z == areaSize.z - 1)
                borderCells.Add(new Vector3Int(targetCell.x - 1, targetCell.y - 1, targetCell.z + 1));

            if (x == 0 && y == areaSize.y - 1 && z == areaSize.z - 1)
                borderCells.Add(new Vector3Int(targetCell.x - 1, targetCell.y + 1, targetCell.z + 1));

            if (x == 0 && y == areaSize.y - 1 && z == 0)
                borderCells.Add(new Vector3Int(targetCell.x - 1, targetCell.y + 1, targetCell.z - 1));

            if (x == areaSize.x - 1 && y == 0 && z == 0)
                borderCells.Add(new Vector3Int(targetCell.x + 1, targetCell.y - 1, targetCell.z - 1));

            if (x == areaSize.x - 1 && y == areaSize.y - 1 && z == 0)
                borderCells.Add(new Vector3Int(targetCell.x + 1, targetCell.y + 1, targetCell.z - 1));

            if (x == areaSize.x - 1 && y == 0 && z == areaSize.z - 1)
                borderCells.Add(new Vector3Int(targetCell.x + 1, targetCell.y - 1, targetCell.z + 1));

            if (x == areaSize.x - 1 && y == areaSize.y - 1 && z == areaSize.z - 1)
                borderCells.Add(new Vector3Int(targetCell.x + 1, targetCell.y + 1, targetCell.z + 1));

            return borderCells;
        }
        #endregion
    }
}
