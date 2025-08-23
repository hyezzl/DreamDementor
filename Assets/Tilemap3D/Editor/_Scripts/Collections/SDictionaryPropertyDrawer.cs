using UnityEditor;

using UnityEngine;

using System;
using System.Collections.Generic;

using Tilemap3D.Collections;

using Tilemap3DEditor.IMGUI;

namespace Tilemap3DEditor.Collections
{
    [CustomPropertyDrawer(typeof(SDictionary<,>), true)]
    public class SDictionaryPropertyDrawer : PropertyDrawerBase
    {
        protected const string KEY_COLLISIONS_FIELD_NAME = "keyCollisions";
        protected const string KEY_TYPENAME_FIELD_NAME = "keyTypeName";
        protected const string VALUE_TYPENAME_FIELD_NAME = "valueTypeName";
        protected const string ENTRIES_FIELD_NAME = "entries";
        protected const string ENTRY_KEY_FIELD_NAME = "key";
        protected const string ENTRY_VALUE_FIELD_NAME = "value";

        protected bool duplicateKeys;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            string keyTypeName = property.FindPropertyRelative(KEY_TYPENAME_FIELD_NAME).stringValue;
            string valueTypeName = property.FindPropertyRelative(VALUE_TYPENAME_FIELD_NAME).stringValue;

            label.text = (property.isExpanded ? " " : "") + ObjectNames.NicifyVariableName(this.fieldInfo.Name) + " <" + keyTypeName + ", " + valueTypeName + ">";

            EditorGUI.BeginProperty(position, label, property);

            BeginPadding(ref position, new Vector4(3, 0, 0, 0));

            property.isExpanded = EditorGUI.Foldout(MoveRect(ref position), property.isExpanded, label, true);

            if (property.isExpanded)
            {
                BeginPadding(ref position, new Vector4(0, 5, 0, 0));

                SDictionaryPropertyDrawerLayoutAttribute.EEntryLayout entryLayout = SDictionaryPropertyDrawerLayoutAttribute.EEntryLayout.Split;
                foreach (Attribute a in fieldInfo.GetCustomAttributes(true))
                {
                    if (a is SDictionaryPropertyDrawerLayoutAttribute attr)
                        entryLayout = attr.entryLayout;
                }

                SerializedProperty entries = property.FindPropertyRelative(ENTRIES_FIELD_NAME);
                SerializedProperty keyCollisions = property.FindPropertyRelative(KEY_COLLISIONS_FIELD_NAME);

                Indent(ref position);

                EditorGUIUtility.labelWidth = 50f;
                int size = EditorGUI.DelayedIntField(MoveRect(ref position), new GUIContent("Size", "The size of the dictionary."), entries.arraySize);
                EditorGUIUtility.labelWidth = originalLabelWidth;
                MoveRect(ref position, -1, 5f);

                size = Mathf.Max(size, 0);

                while (size > entries.arraySize)
                    entries.arraySize++;

                while (size < entries.arraySize && size >= 0)
                    entries.arraySize--;

                bool foundDuplicateKeys = false;
                for (int i = 0; i < entries.arraySize; i++)
                {
                    if (i == 0)
                        DrawLine(ref position, -1, 2);

                    SerializedProperty entry = entries.GetArrayElementAtIndex(i);
                    SerializedProperty key = entry.FindPropertyRelative(ENTRY_KEY_FIELD_NAME);
                    SerializedProperty value = entry.FindPropertyRelative(ENTRY_VALUE_FIELD_NAME);

                    if (entryLayout == SDictionaryPropertyDrawerLayoutAttribute.EEntryLayout.Split)
                    {
                        float seperatorWidth = 2f;
                        float buttonWidth = 20f;
                        float keyWidth = (position.width - buttonWidth - seperatorWidth - (horizontalSpacing * 2)) / 2f;
                        float keyHeight = EditorGUI.GetPropertyHeight(key);
                        float valueWidth = keyWidth;
                        float valueHeight = EditorGUI.GetPropertyHeight(value);

                        if (keyHeight > valueHeight)
                            valueHeight = keyHeight;
                        else
                            keyHeight = valueHeight;

                        if (keyCollisions != null)
                        {
                            for (int j = 0; j < keyCollisions.arraySize; ++j)
                            {
                                int keyCollisionIndex = keyCollisions.GetArrayElementAtIndex(j).intValue;
                                if (keyCollisionIndex == i)
                                {
                                    foundDuplicateKeys = true;
                                    EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, keyHeight), new Color(1, 0, 0, 0.15f));
                                    GUI.backgroundColor = Color.red;
                                    break;
                                }
                                else
                                    GUI.backgroundColor = originalBackgroundColor;
                            }
                        }

                        BeginHorizontal(ref position);

                        bool keyHasVisibleChildren = key != null && key.hasVisibleChildren;
                        if (keyHasVisibleChildren)
                        {
                            EditorGUIUtility.labelWidth = indentSpacing;
                        }

                        EditorGUI.PropertyField(
                            MoveRect(ref position, keyWidth, keyHeight),
                            key,
                            keyHasVisibleChildren ? new GUIContent(keyTypeName) : GUIContent.none,
                            true
                        );

                        if (keyHasVisibleChildren)
                        {
                            EditorGUIUtility.labelWidth = originalLabelWidth;
                        }

                        DrawLine(ref position, new Color(0, 0, 0, 0.5f), seperatorWidth, keyHeight);

                        bool valueHasVisibleChildren = value != null && value.hasVisibleChildren;
                        if (valueHasVisibleChildren)
                        {
                            valueWidth -= indentSpacing;
                            EditorGUIUtility.labelWidth = indentSpacing;
                        }

                        EditorGUI.PropertyField(
                            MoveRect(ref position, valueWidth, valueHeight),
                            value,
                            valueHasVisibleChildren ? new GUIContent(valueTypeName) : GUIContent.none,
                            true
                        );

                        GUI.backgroundColor = Color.red;
                        bool xBtnClicked = GUI.Button(MoveRect(ref position, buttonWidth), "x");
                        GUI.backgroundColor = originalBackgroundColor;

                        if (valueHasVisibleChildren)
                        {
                            valueWidth += indentSpacing;
                            EditorGUIUtility.labelWidth = originalLabelWidth;
                        }

                        if (xBtnClicked)
                        {
                            GUI.FocusControl(null);
                            entries.DeleteArrayElementAtIndex(i);
                        }

                        EndHorizontal(ref position);
                    }
                    else if (entryLayout == SDictionaryPropertyDrawerLayoutAttribute.EEntryLayout.List)
                    {
                        float buttonWidth = 20f;
                        float keyLabelWidth = position.width - buttonWidth - (horizontalSpacing * 2);
                        float keyHeight = EditorGUI.GetPropertyHeight(key);
                        float valueHeight = EditorGUI.GetPropertyHeight(value);

                        bool isDuplicateKey = false;
                        if (keyCollisions != null)
                        {
                            for (int j = 0; j < keyCollisions.arraySize; ++j)
                            {
                                int keyCollisionIndex = keyCollisions.GetArrayElementAtIndex(j).intValue;
                                if (keyCollisionIndex == i)
                                {
                                    foundDuplicateKeys = true;
                                    isDuplicateKey = true;
                                    GUI.backgroundColor = Color.red;
                                    EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, keyHeight), new Color(1, 0, 0, 0.15f));
                                    break;
                                }
                                else
                                    GUI.backgroundColor = originalBackgroundColor;
                            }
                        }

                        BeginHorizontal(ref position);

                        EditorGUI.LabelField(MoveRect(ref position, keyLabelWidth), key.displayName, EditorStyles.boldLabel);

                        GUI.backgroundColor = Color.red;
                        bool xBtnClicked = GUI.Button(MoveRect(ref position, buttonWidth), "x");
                        if (!isDuplicateKey)
                            GUI.backgroundColor = originalBackgroundColor;

                        EndHorizontal(ref position);

                        EditorGUI.PropertyField(
                            MoveRect(ref position, -1, keyHeight),
                            key,
                            GUIContent.none,
                            true
                        );

                        MoveRect(ref position, -1, 1);

                        EditorGUI.PropertyField(
                            MoveRect(ref position, -1, valueHeight),
                            value,
                            new GUIContent(value.displayName),
                            true
                        );

                        if (xBtnClicked)
                        {
                            GUI.FocusControl(null);
                            entries.DeleteArrayElementAtIndex(i);
                        }
                    }

                    MoveRect(ref position, -1, 1);
                    DrawLine(ref position, -1, 1);
                }
                duplicateKeys = foundDuplicateKeys;

                MoveRect(ref position, -1, 5f);
                GUI.backgroundColor = originalBackgroundColor;
                if (GUI.Button(MoveRect(ref position), "Add"))
                {
                    entries.arraySize++;
                }

                Indent(ref position, -1);

                EndPadding(ref position, new Vector4(0, 5, 0, 0));
                Space(ref position, 2); // <- needed because there are no more controls after this in the foldOut and it looks odd without some space at the end
                GUI.backgroundColor = new Color(1, 1, 1, 0);
                EditorGUI.HelpBox(new Rect(startPosition, new Vector2(position.width, height)), "", MessageType.None);
                GUI.backgroundColor = originalBackgroundColor;
            }

            if (duplicateKeys)
            {
                MoveRect(ref position, -1, 1);

                // there are key collisions, so we render a warning box.
                EditorGUI.HelpBox(
                    MoveRect(ref position, -1, EditorGUIUtility.singleLineHeight * 3),
                    "There are duplicate keys in the dictionary. Duplicates will be excluded from the dictionary.",
                    MessageType.Warning
                );
            }

            EditorGUI.EndProperty();

            if (property.isExpanded)
                Space(ref position, 5);
        }
    }
}
