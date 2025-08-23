using UnityEditor;

using UnityEngine;

using System;
using System.Collections.Generic;

namespace Tilemap3DEditor
{
    /// <summary>
    /// A base Inspector class that automatically draws serialized properties the way Unity normal does.
    /// This is useful because sometimes you just want to implement a custom inspector and just add/remove a few things
    /// without having to manually redraw all the other serialized properties.
    /// </summary>
    public abstract class Inspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            SerializedProperty begin = serializedObject.GetIterator();
            if (begin != null)
            {
                SerializedProperty it = begin.Copy();
                if (it.NextVisible(true))
                {
                    do
                    {
                        if (!AutoDrawPropertyGuard(it.Copy())) 
                            continue;

                        OnDrawProperty(it, new GUIContent(it.displayName));
                    }
                    while (it.NextVisible(false));
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        protected virtual bool AutoDrawPropertyGuard(SerializedProperty serializedProperty)
        {
            return true;
        }

        protected virtual void OnDrawProperty(SerializedProperty serializedProperty, GUIContent label, bool includeChildren = true)
        {
            if (serializedProperty.name == "m_Script")
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedProperty, label, includeChildren);
                GUI.enabled = true;
                GUILayout.Space(5);
            }
            else
                EditorGUILayout.PropertyField(serializedProperty, label, includeChildren);
        }

        protected void DrawHorizontalLine(int height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }
    }
}
