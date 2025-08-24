using UnityEngine;

namespace Tilemap3D.Collections
{
    public class SDictionaryPropertyDrawerLayoutAttribute : PropertyAttribute
    {
        public enum EEntryLayout { Split, List }

        public readonly EEntryLayout entryLayout;

        public SDictionaryPropertyDrawerLayoutAttribute(EEntryLayout entryLayout = EEntryLayout.Split)
        {
            this.entryLayout = entryLayout;
        }
    }
}
