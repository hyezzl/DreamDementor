using UnityEditor;

using UnityEngine;
using UnityEngine.Rendering;

using System.Collections;
using System.Collections.Generic;

using UnityObject = UnityEngine.Object;
using System;

namespace Tilemap3DEditor.Utilities
{
    public static class MeshCombiner
    {
        public static GameObject CombineMeshObjects(MeshFilter[] meshFilters, string newObjectName, Transform newObjectParent)
        {
            if (meshFilters == null || meshFilters.Length == 0)
                return null;

            // calculate the positionShift which is used to shift the origin of the meshes
            // so that they aren't far away from the origin in the combinedObject
            Vector3 positionShift;
            Vector3 parentPosition = newObjectParent == null ? Vector3.zero : newObjectParent.position;
            Vector3 closestPosition = parentPosition;
            float closestDistance = float.PositiveInfinity;
            foreach (MeshFilter meshFilter in meshFilters)
            {
                float distance = Vector3.Distance(parentPosition, meshFilter.transform.position);
                if (distance < closestDistance)
                {
                    closestPosition = meshFilter.transform.position;
                    closestDistance = distance;
                }
            }
            positionShift = parentPosition - closestPosition;

            ArrayList materials = new ArrayList();
            ArrayList combineInstanceArrays = new ArrayList();
            foreach (MeshFilter meshFilter in meshFilters)
            {
                MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

                if (meshRenderer == null || meshFilter.sharedMesh == null || meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
                    continue;

                for (int i = 0; i < meshFilter.sharedMesh.subMeshCount; i++)
                {
                    int materialIndex = ContainsMaterial(materials, meshRenderer.sharedMaterials[i]);
                    if (materialIndex == -1)
                    {
                        materials.Add(meshRenderer.sharedMaterials[i]);
                        materialIndex = materials.Count - 1;
                    }

                    ArrayList combineInstanceArrayList = new ArrayList();
                    combineInstanceArrays.Add(combineInstanceArrayList);

                    CombineInstance combineInstance = new CombineInstance();
                    Vector3 oldPosition = meshRenderer.transform.position;
                    meshRenderer.transform.position += positionShift;
                    combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                    meshRenderer.transform.position = oldPosition;
                    combineInstance.subMeshIndex = i;
                    combineInstance.mesh = meshFilter.sharedMesh;
                    (combineInstanceArrays[materialIndex] as ArrayList).Add(combineInstance);
                }
            }

            // create gameobject, mesh filter and renderer
            GameObject combinedObject = new GameObject(newObjectName);

            if (newObjectParent != null)
            {
                combinedObject.transform.SetParent(newObjectParent);
                combinedObject.transform.position -= positionShift;
            }

            MeshFilter combinedObjectMeshFilter = combinedObject.AddComponent<MeshFilter>();
            MeshRenderer combinedObjectMeshRenderer = combinedObject.AddComponent<MeshRenderer>();

            // combine by material index into per-material meshes and create the CombineInstance array
            Mesh[] tempMeshes = new Mesh[materials.Count];
            CombineInstance[] combineInstances = new CombineInstance[materials.Count];

            for (int i = 0; i < materials.Count; i++)
            {
                CombineInstance[] combineInstanceArray = (combineInstanceArrays[i] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];

                int combineInstanceArrayVertexCount = 0;
                for (int j = 0; j < combineInstanceArray.Length; j++)
                    combineInstanceArrayVertexCount += combineInstanceArray[j].mesh.vertexCount;

                IndexFormat combineInstanceArrayVertexBufferFormat = IndexFormat.UInt16;
                if (combineInstanceArrayVertexCount > UInt16.MaxValue)
                    combineInstanceArrayVertexBufferFormat = IndexFormat.UInt32;

                tempMeshes[i] = new Mesh() { indexFormat = combineInstanceArrayVertexBufferFormat };
                tempMeshes[i].CombineMeshes(combineInstanceArray, true, true);

                combineInstances[i] = new CombineInstance();
                combineInstances[i].mesh = tempMeshes[i];
                combineInstances[i].subMeshIndex = 0;
            }

            int combineInstancesVertexCount = 0;
            for (int k = 0; k < combineInstances.Length; k++)
                combineInstancesVertexCount += combineInstances[k].mesh.vertexCount;

            IndexFormat combineInstancesVertexBufferFormatBufferFormat = IndexFormat.UInt16;
            if (combineInstancesVertexCount > UInt16.MaxValue)
                combineInstancesVertexBufferFormatBufferFormat = IndexFormat.UInt32;

            combinedObjectMeshFilter.sharedMesh = new Mesh() { indexFormat = combineInstancesVertexBufferFormatBufferFormat };
            combinedObjectMeshFilter.sharedMesh.CombineMeshes(combineInstances, false, false);
            combinedObjectMeshFilter.sharedMesh.name = newObjectName;

            // destroy temporary meshes
            foreach (Mesh tempMesh in tempMeshes)
            {
                tempMesh.Clear();
                UnityObject.DestroyImmediate(tempMesh);
            }

            // assign materials
            Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
            combinedObjectMeshRenderer.materials = materialsArray;

            return combinedObject;
        }

        private static int ContainsMaterial(ArrayList searchList, Material material)
        {
            for (int i = 0; i < searchList.Count; i++)
            {
                Material mat = searchList[i] as Material;

                if (mat == null)
                    continue;

                if (mat == material)
                    return i;
            }

            return -1;
        }
    }
}
