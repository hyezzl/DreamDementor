using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

using Tilemap3D;

namespace Tilemap3DEditor
{
    [CustomEditor(typeof(RulesetTile))]
    public class RulesetTileInspector : Inspector
    {
        private SerializedProperty spRules;
        private SerializedProperty spDefaultTile;

        private RulesetTile rulesetTile;

        protected override void OnEnable()
        {
            base.OnEnable();

            spDefaultTile = serializedObject.FindProperty(nameof(RulesetTile.defaultTile));
            spRules = serializedObject.FindProperty(nameof(RulesetTile.rules));

            rulesetTile = target as RulesetTile;
        }

        public override void OnInspectorGUI()
        {
            // do nothing, draw using UIToolkit instead
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new VisualElement();
            container.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetPaths.TILEMAP3D_EDITOR_SCRIPTS_DIR + "RulesetTileInspector.uss"));

            PropertyField fieldDefaultTile = new PropertyField(spDefaultTile);
            fieldDefaultTile.BindProperty(spDefaultTile);
            fieldDefaultTile.RegisterValueChangeCallback(evt =>
            {
                if (spDefaultTile.objectReferenceValue == null)
                    fieldDefaultTile.Q(null, "unity-object-field__input")?.AddToClassList("background-red");
                else
                    fieldDefaultTile.Q(null, "unity-object-field__input")?.RemoveFromClassList("background-red");
            });

            if (spDefaultTile.objectReferenceValue == null)
                fieldDefaultTile.Q(null, "unity-object-field__input")?.AddToClassList("background-red");
            else
                fieldDefaultTile.Q(null, "unity-object-field__input")?.RemoveFromClassList("background-red");

            PropertyField fieldRules = new PropertyField(spRules);
            fieldRules.Bind(serializedObject);

            Button btnValidateRulesetTilesInScene = new Button(rulesetTile.ValidateRulesetTilesInScene) 
            { 
                text = "Revalidate Ruleset Tiles In Scene",
                style = { alignSelf = Align.FlexStart }
            };

            container.Add(fieldDefaultTile);
            container.Add(fieldRules);
            container.Add(btnValidateRulesetTilesInScene);

            return container;
        }
    }
}
