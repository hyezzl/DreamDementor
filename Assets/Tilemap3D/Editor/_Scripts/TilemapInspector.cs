using UnityEditor;

using UnityEngine;

using Tilemap3D;

namespace Tilemap3DEditor
{
    [CustomEditor(typeof(Tilemap))]
    public class TilemapInspector : Inspector
    {
        private Tilemap tilemap;

        protected override void OnEnable()
        {
            base.OnEnable();

            tilemap = target as Tilemap;
        }

        public override void OnInspectorGUI()
        {
            if (tilemap != null && tilemap.CellLayout != Tilemap.ECellLayout.Rectangle)
            {
                Debug.LogWarning($"Unsupported Grid Cell Layout '{tilemap.CellLayout}'", tilemap);
                tilemap.CellLayout = Tilemap.ECellLayout.Rectangle;
            }

            base.OnInspectorGUI();

            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("Adjust Tile Positions")))
            {
                TilemapEditorUtility.AdjustTilePositions(tilemap);
            }
        }
    }
}
