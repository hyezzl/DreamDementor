using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

using Tilemap3D;

namespace Tilemap3DEditor
{
    [CustomEditor(typeof(RandomizerTile))]
    public class RandomizerTileInspector : Inspector
    {
        private RandomizerTile randomizerTile;

        protected override void OnEnable()
        {
            base.OnEnable();

            randomizerTile = target as RandomizerTile;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button(new GUIContent("ReRandomize Tiles In Scene", "ReRandomize any tiles in the scene that originated from this asset.")))
            {
                randomizerTile.ReRandomizeTilesInScene();
            }
        }
    }
}
