using UnityEditor;

using UnityEngine;

using Tilemap3D;

namespace Tilemap3DEditor
{
    [CustomEditor(typeof(TileLayer))]
    public class TileLayerInspector : Inspector
    {
        private TileLayer tileLayer;

        protected override void OnEnable()
        {
            base.OnEnable();

            tileLayer = target as TileLayer;
        }

        public override void OnInspectorGUI()
        {
            if (tileLayer != null && tileLayer.transform.localScale != Vector3.one)
            {
                EditorGUILayout.HelpBox(
                    "Warning: You should not change the local scale of the layer object. The grid system does not scale with the layer.",
                    MessageType.Warning
                );
            }

            base.OnInspectorGUI();
        }
    }
}
