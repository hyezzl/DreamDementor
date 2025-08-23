using UnityEditor;

using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Tilemap3DEditor.Utilities
{
    public static class EditorUtil
    {
        /// <summary>
        /// Instantiate a prefab under each of the given <paramref name="parents"/>.
        /// </summary>
        /// <returns>The list of gameobjects that were instantiated.</returns>
        public static List<GameObject> CreatePrefabUnderParents(string name, string assetPath, GameObject[] parents, bool unpack = false, bool registerCreateUndo = true)
        {
            List<GameObject> instances = new List<GameObject>();
            if (parents == null || parents.Length == 0)
                instances.Add(CreatePrefabUnderParent(name, assetPath, null, unpack, registerCreateUndo));
            else
            {
                int undoId = Undo.GetCurrentGroup();

                foreach (GameObject parent in parents)
                    instances.Add(CreatePrefabUnderParent(name, assetPath, parent, unpack, registerCreateUndo));

                Undo.CollapseUndoOperations(undoId);
            }

            return instances;
        }

        /// <summary>
        /// Instantiate a prefab under the given <paramref name="parent"/>.
        /// </summary>
        /// <returns>The list of gameobjects that were instantiated.</returns>
        public static GameObject CreatePrefabUnderParent(string name, string assetPath, GameObject parent, bool unpack = false, bool registerCreateUndo = true)
        {
            GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>(assetPath));
            prefab.name = name;

            if (parent != null)
                prefab.transform.SetParent(parent.transform, false);

            if (unpack)
                PrefabUtility.UnpackPrefabInstance(prefab, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            if (registerCreateUndo)
                Undo.RegisterCreatedObjectUndo(prefab, "Create " + prefab.name);

            return prefab;
        }

        /// <summary>
        /// Instantiate a gameobject under each of the given <paramref name="parents"/> and attach given components.
        /// </summary>
        /// <returns>The list of gameobjects that were instantiated.</returns>
        public static List<GameObject> CreateGameObjectUnderParents(string name, GameObject[] parents, bool registerCreateUndo = true, params System.Type[] componentTypes)
        {
            List<GameObject> instances = new List<GameObject>();

            if (parents == null || parents.Length == 0)
                instances.Add(CreateGameObjectUnderParent(name, null, registerCreateUndo, componentTypes));
            else
            {
                int undoId = Undo.GetCurrentGroup();

                foreach (GameObject parent in parents)
                    instances.Add(CreateGameObjectUnderParent(name, parent, registerCreateUndo, componentTypes));

                Undo.CollapseUndoOperations(undoId);
            }

            return instances;
        }

        /// <summary>
        /// Instantiate a gameobject under the given <paramref name="parent"/> and attach given components.
        /// </summary>
        /// <returns>The gameobject that was instantiated.</returns>
        public static GameObject CreateGameObjectUnderParent(string name, GameObject parent, bool registerCreateUndo = true, params System.Type[] componentTypes)
        {
            GameObject gameObject = new GameObject(name);
            foreach (System.Type type in componentTypes)
                gameObject.AddComponent(type);

            if (parent != null)
                gameObject.transform.SetParent(parent.transform, false);

            if (registerCreateUndo)
                Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);

            return gameObject;
        }
    }
}
