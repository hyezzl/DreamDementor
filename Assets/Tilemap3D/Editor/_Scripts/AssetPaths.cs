
namespace Tilemap3DEditor
{
    /// <summary>
    /// A utility class containing constants of various asset paths within 
    /// the Tilemap3D and Tilemap3DEditor assemblies.
    /// </summary>
    public static class AssetPaths
    {
        // root directory for the Tilemap 3D assemblies, change this if you decide to move this directory elsewhere.
        public static readonly string TILEMAP3D_DIR = "Assets/Tilemap3D/";

        public static readonly string TILEMAP3D_RUNTIME_DIR = TILEMAP3D_DIR + "_Runtime/";

        public static readonly string TILEMAP3D_EDITOR_DIR = TILEMAP3D_DIR + "Editor/";
        public static readonly string TILEMAP3D_EDITOR_SCRIPTS_DIR = TILEMAP3D_EDITOR_DIR + "_Scripts/";
        public static readonly string TILEMAP3D_EDITOR_TEXTURES_DIR = TILEMAP3D_EDITOR_DIR + "Textures/";
    }
}
