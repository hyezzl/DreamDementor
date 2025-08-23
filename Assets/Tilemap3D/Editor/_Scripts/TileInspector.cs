using UnityEditor;

using UnityEngine;

using Tilemap3D;

namespace Tilemap3DEditor
{
    [CustomEditor(typeof(Tile))]
    public class TileInspector : Inspector
    {
        protected override void OnDrawProperty(SerializedProperty serializedProperty, GUIContent label, bool includeChildren = true)
        {
            if (serializedProperty.name == "tilemap" || serializedProperty.name == "layer" || serializedProperty.name == "gridCellPosition")
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedProperty, label, includeChildren);
                GUI.enabled = true;
            }
            else
                base.OnDrawProperty(serializedProperty, label, includeChildren);
        }
    }
}
