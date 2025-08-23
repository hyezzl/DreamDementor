using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Tilemap3D
{
    public interface IPaletteTile
    {
        public PrefabData GetPrefabData(TileContext ctx = null);

        public class PrefabData
        {
            public GameObject prefab;
            public Quaternion ruleRotation = Quaternion.identity;
            public Vector3 ruleScale = Vector3.one;
        }
    }
}
