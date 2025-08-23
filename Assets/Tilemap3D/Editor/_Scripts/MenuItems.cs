using UnityEditor;

using UnityEngine;

using Tilemap3DEditor.Utilities;
using Tilemap3D;

namespace Tilemap3DEditor
{
    public static class MenuItems
    {
        public static readonly string PREFABS_DIR = AssetPaths.TILEMAP3D_RUNTIME_DIR + "Prefabs/";
        public static readonly string TILEMAP_PREFAB_PATH = PREFABS_DIR + "Tilemap.prefab";
        public static readonly string TILE_LAYER_PREFAB_PATH = PREFABS_DIR + "TileLayer.prefab";

        [MenuItem("GameObject/Tilemap3D/Tilemap")]
        static void CreateTilemap()
        {
            EditorUtil.CreatePrefabUnderParents("Tilemap", TILEMAP_PREFAB_PATH, Selection.gameObjects, true);
        }

        [MenuItem("GameObject/Tilemap3D/TileLayer")]
        static void CreateTileLayer()
        {
            if (Selection.gameObjects.Length > 0)
            {
                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    GameObject selectedObject = Selection.gameObjects[i];
                    if (selectedObject.GetComponentInParent<Tilemap>() == null)
                        EditorUtil.CreatePrefabUnderParent("Tilemap", TILEMAP_PREFAB_PATH, selectedObject, true, true);
                    else
                        EditorUtil.CreatePrefabUnderParent("TileLayer", TILE_LAYER_PREFAB_PATH, selectedObject, true);
                }
            }
            else
                CreateTilemap();
        }
    }
}
