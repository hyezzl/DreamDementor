using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Tilemap3D;
using Tilemap3D.Collections;

using Tilemap3DEditor.Utilities;

namespace Tilemap3DEditor
{
    [Serializable]
    public class Tilemap3DEditorWindow : EditorWindow
    {
        private const string WINDOW_TITLE = "Tilemap3D";
        private const string EDITOR_PREF_KEY_GRID_POS_Y = "Tilemap3DEditorWindow.gridPositionY";
        private const string EDITOR_PREF_KEY_GRID_PLANE_EXTENTS = "Tilemap3DEditorWindow.gridGizmoExtents";
        private const string EDITOR_PREF_KEY_GRID_COLOR = "Tilemap3DEditorWindow.gridGizmoColor";
        private const string EDITOR_PREF_KEY_PLACEMENT_TOOL = "Tilemap3DEditorWindow.placementTool";
        private const string EDITOR_PREF_KEY_PLACEMENT_GIZMO_COLOR = "Tilemap3DEditorWindow.placementGizmoColor";
        private const string EDITOR_PREF_KEY_UNPACK_PREFAB = "Tilemap3DEditorWindow.upackPrefab";
        private const string EDITOR_PREF_KEY_PLACEMENT_OFFSET = "Tilemap3DEditorWindow.placementOffset";
        private const string EDITOR_PREF_KEY_PLACEMENT_ROTATION = "Tilemap3DEditorWindow.placementRotation";
        private const string EDITOR_PREF_KEY_PLACEMENT_SCALE = "Tilemap3DEditorWindow.placementScale";
        private const string EDITOR_PREF_KEY_ERASER_SIZE = "Tilemap3DEditorWindow.eraserSize";
        private const string EDITOR_PREF_KEY_ERASER_USE_PLACEMENT_OFFSETS = "Tilemap3DEditorWindow.eraserUsePlacementOffsets";
        private const string EDITOR_PREF_KEY_ERASER_OFFSET_FLAG = "Tilemap3DEditorWindow.eraserOffsetFlag";
        private const string EDITOR_PREF_KEY_ERASER_ROTATION_OFFSET_FLAG = "Tilemap3DEditorWindow.eraserRotationOffsetFlag";
        private const string EDITOR_PREF_KEY_ERASER_SCALE_OFFSET_FLAG = "Tilemap3DEditorWindow.eraserScaleOffsetFlag";
        private const string EDITOR_PREF_KEY_SELECTION_MODE = "Tilemap3DEditorWindow.selectionMode";
        private const string EDITOR_PREF_KEY_WAND_FILTER = "Tilemap3DEditorWindow.wandFilter";
        private const string EDITOR_PREF_KEY_MESH_COMBINER_DESTROY_TILES = "Tilemap3DEditorWindow.meshCombinerDestroyTiles";
        private const string EDITOR_PREF_KEY_PREFAB_PALETTE_PATH = "Tilemap3DEditorWindow.prefabPalettePath";
        private readonly Vector2Int DEFAULT_GRID_GIZMO_EXTENTS = new Vector2Int(50, 50);
        private readonly Color DEFAULT_GRID_GIZMO_COLOR = new Color(0.376f, 0.49f, 0.54f, 0.25f);

        #region GUI Member References
        private Toolbar toolbarEditMode;
        private ObjectField fieldTilemap;
        private ObjectField fieldTileLayer;
        private IntegerField fieldGridPosX;
        private IntegerField fieldGridPosY;
        private IntegerField fieldGridPosZ;
        private Vector2IntField fieldGridGizmoExtents;
        private ColorField fieldGridGizmoColor;
        private VisualElement vePlacementOptions;
        private Toolbar toolbarPaintTool;
        private ColorField fieldPlacementGizmoColor;
        private Toggle toggleUnpackPrefab;
        private Vector3Field fieldPlacementOffset;
        private Vector3Field fieldPlacementRotation;
        private Vector3Field fieldPlacementScale;
        private VisualElement veEraserOptions;
        private Vector3IntField fieldEraserSize;
        private Toggle toggleEraserUsePlacementOffsets;
        private Toggle toggleEraserOffset;
        private Toggle toggleEraserRotationOffset;
        private Toggle toggleEraserScaleOffset;
        private VisualElement veSelectionOptions;
        private Button btnSelectAllInLayer;
        private Button btnClearSelection;
        private Toolbar toolbarSelectionMode;
        private Label lblDefaultSelectionTips;
        private EnumField fieldWandFilter;
        private VisualElement veMeshCombiner;
        private Toggle toggleMCDestroyTiles;
        private Button btnCombineMeshes;
        private VisualElement vePalette;
        private ObjectField fieldPalette;
        private SliderInt sliderPaletteTileWidth;
        private SliderInt sliderPaletteTileHeight;
        private Button btnPaletteTilePreviewRefresh;
        private TextField fieldPaletteTileFilter;
        private PaletteView paletteView;
        #endregion

        private static Tilemap3DEditorWindow instance;
        private EEditMode editMode;
        private EPlacementMode placementMode;
        private ESelectionMode selectionMode;
        private EWandFilter wandFilter = EWandFilter.SamePaletteTile;
        private bool shouldDrawSceneGizmos = true;
        private Vector3 targetPosition;
        private bool isDrawingIMGUI = true;
        private Vector3Int? mouseDownGridCell;

        public enum EEditMode { Default, Paint, Erase, Select }

        public enum EPlacementMode { Default, Bucket }

        public enum ESelectionMode { Default, Wand }

        public enum EWandFilter { Any, SameRuleset, SamePaletteTile }

        [MenuItem("Tools/Tilemap3D/" + WINDOW_TITLE + " Editor")]
        public static void OpenWindow()
        {
            instance = GetWindow<Tilemap3DEditorWindow>();
            instance.Open();
        }

        public void Open()
        {
            minSize = new Vector2(100, 100);
            titleContent = new GUIContent(" " + WINDOW_TITLE);

            Show();
        }

        protected void OnEnable()
        {
            instance = this;
        }

        #region CreateGUI
        protected void CreateGUI()
        {
            rootVisualElement.Clear();

            // build GUI from UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetPaths.TILEMAP3D_EDITOR_SCRIPTS_DIR + "Tilemap3DEditorWindow.uxml");
            visualTree.CloneTree(rootVisualElement);

            // add stylesheet 
            StyleSheet ss = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetPaths.TILEMAP3D_EDITOR_SCRIPTS_DIR + "Tilemap3DEditorWindow.uss");
            rootVisualElement.styleSheets.Add(ss);

            // gather references ...
            toolbarEditMode = rootVisualElement.Q<Toolbar>("toolbarEditMode");
            fieldTilemap = rootVisualElement.Q<ObjectField>("fieldTilemap");
            fieldTileLayer = rootVisualElement.Q<ObjectField>("fieldTileLayer");
            fieldGridPosX = rootVisualElement.Q<IntegerField>("fieldGridPosX");
            fieldGridPosY = rootVisualElement.Q<IntegerField>("fieldGridPosY");
            fieldGridPosZ = rootVisualElement.Q<IntegerField>("fieldGridPosZ");
            fieldGridGizmoExtents = rootVisualElement.Q<Vector2IntField>("fieldGridGizmoExtents");
            fieldGridGizmoColor = rootVisualElement.Q<ColorField>("fieldGridGizmoColor");
            fieldPlacementGizmoColor = rootVisualElement.Q<ColorField>("fieldPlacementGizmoColor");
            vePlacementOptions = rootVisualElement.Q<VisualElement>("vePlacementOptions");
            toolbarPaintTool = rootVisualElement.Q<Toolbar>("toolbarPaintTool");
            toggleUnpackPrefab = rootVisualElement.Q<Toggle>("toggleUnpackPrefab");
            fieldPlacementOffset = rootVisualElement.Q<Vector3Field>("fieldPlacementOffset");
            fieldPlacementRotation = rootVisualElement.Q<Vector3Field>("fieldPlacementRotation");
            fieldPlacementScale = rootVisualElement.Q<Vector3Field>("fieldPlacementScale");
            veEraserOptions = rootVisualElement.Q<VisualElement>("veEraserOptions");
            fieldEraserSize = rootVisualElement.Q<Vector3IntField>("fieldEraserSize");
            toggleEraserUsePlacementOffsets = rootVisualElement.Q<Toggle>("toggleEraserUsePlacementOffsets");
            toggleEraserOffset = rootVisualElement.Q<Toggle>("toggleEraserOffset");
            toggleEraserRotationOffset = rootVisualElement.Q<Toggle>("toggleEraserRotationOffset");
            toggleEraserScaleOffset = rootVisualElement.Q<Toggle>("toggleEraserScaleOffset");
            veSelectionOptions = rootVisualElement.Q<VisualElement>("veSelectionOptions");
            btnSelectAllInLayer = rootVisualElement.Q<Button>("btnSelectAllInLayer");
            btnClearSelection = rootVisualElement.Q<Button>("btnClearSelection");
            toolbarSelectionMode = rootVisualElement.Q<Toolbar>("toolbarSelectionTool");
            lblDefaultSelectionTips = rootVisualElement.Q<Label>("lblDefaultSelectionTips");
            fieldWandFilter = rootVisualElement.Q<EnumField>("fieldWandFilter");
            veMeshCombiner = rootVisualElement.Q<VisualElement>("veMeshCombiner");
            toggleMCDestroyTiles = rootVisualElement.Q<Toggle>("toggleMCDestroyTiles");
            btnCombineMeshes = rootVisualElement.Q<Button>("btnCombineMeshes");
            vePalette = rootVisualElement.Q<VisualElement>("vePalette");
            fieldPalette = rootVisualElement.Q<ObjectField>("fieldPalette");
            sliderPaletteTileWidth = rootVisualElement.Q<SliderInt>("sliderPaletteTileWidth");
            sliderPaletteTileHeight = rootVisualElement.Q<SliderInt>("sliderPaletteTileHeight");
            btnPaletteTilePreviewRefresh = rootVisualElement.Q<Button>("btnPaletteTilePreviewRefresh");
            fieldPaletteTileFilter = rootVisualElement.Q<TextField>("fieldPaletteTileFilter");
            paletteView = rootVisualElement.Q<PaletteView>("paletteView");

            // setup GUI ...

            // matches valid enum names (word that doesn't start with number and contains only a-z, A-Z, 0-9, and _
            string regexValidEnum = @"[a-zA-Z_](?:[a-zA-Z0-9_]+)*";

            ToolbarToggle[] toolbarEditModeToggles = toolbarEditMode.Children().Cast<ToolbarToggle>().ToArray();
            if (toolbarEditModeToggles != null && toolbarEditModeToggles.Length > 0)
            {
                foreach (ToolbarToggle toggle in toolbarEditModeToggles)
                {
                    string editModeStr = "";
                    if (toggle != null && toggle.text != null)
                        editModeStr = Regex.Match(toggle.text, regexValidEnum).Value;

                    EEditMode editMode = EEditMode.Default;
                    try
                    {
                        editMode = Enum.Parse<EEditMode>(editModeStr);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error parsing EEditMode enum from toggle text. " + e.Message);
                    }
                    toggle.userData = editMode;

                    toggle.RegisterValueChangedCallback((evt) =>
                    {
                        if (evt.newValue == evt.previousValue)
                            return;

                        EditMode = editMode;
                    });
                }

                EditMode = EEditMode.Paint;
            }

            fieldTilemap.Q(null, "unity-object-field__selector").SetEnabled(false);
            fieldTilemap.objectType = typeof(Tilemap);

            fieldTileLayer.Q(null, "unity-object-field__selector").SetEnabled(false);
            fieldTileLayer.objectType = typeof(TileLayer);

            fieldGridPosY.tooltip = "[Space] : ++\n[C] : --";
            fieldGridPosY.RegisterValueChangedCallback(evt => 
            {
                UpdateTilemapGridGizmoProperties();
            });

            fieldGridGizmoExtents.RegisterValueChangedCallback(evt => 
            {
                fieldGridGizmoExtents.SetValueWithoutNotify(new Vector2Int(
                    evt.newValue.x < 0 ? 0 : evt.newValue.x,
                    evt.newValue.y < 0 ? 0 : evt.newValue.y
                ));

                UpdateTilemapGridGizmoProperties();
            });
            fieldGridGizmoExtents.labelElement?.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
            {
                evt.menu.AppendAction("Reset", (e) => { fieldGridGizmoExtents.value = DEFAULT_GRID_GIZMO_EXTENTS; });
            }));

            fieldGridGizmoColor.RegisterValueChangedCallback(evt =>
            {
                UpdateTilemapGridGizmoProperties();
            });
            fieldGridGizmoColor.labelElement?.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
            {
                evt.menu.AppendAction("Reset", (e) => { fieldGridGizmoColor.value = DEFAULT_GRID_GIZMO_COLOR; });
            }));

            ToolbarToggle[] toolbarPaintToolToggles = toolbarPaintTool.Children().Cast<ToolbarToggle>().ToArray();
            if (toolbarPaintToolToggles != null && toolbarPaintToolToggles.Length > 0)
            {
                foreach (ToolbarToggle toggle in toolbarPaintToolToggles)
                {
                    string placementModeStr = "";
                    if (toggle != null && toggle.text != null)
                        placementModeStr = Regex.Match(toggle.text, regexValidEnum).Value;

                    EPlacementMode placementMode = EPlacementMode.Default;
                    try
                    {
                        placementMode = Enum.Parse<EPlacementMode>(placementModeStr);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error parsing EPlacementMode enum from toggle text. " + e.Message);
                    }
                    toggle.userData = placementMode;

                    toggle.RegisterValueChangedCallback((evt) =>
                    {
                        if (evt.newValue == evt.previousValue)
                            return;

                        PlacementMode = placementMode;
                    });
                }

                PlacementMode = EPlacementMode.Default;
            }

            fieldPlacementOffset.labelElement?.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
            {
                evt.menu.AppendAction("Reset", (e) => { fieldPlacementOffset.value = Vector3.zero; });
            }));

            fieldPlacementRotation.labelElement?.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
            {
                evt.menu.AppendAction("Reset", (e) => { fieldPlacementRotation.value = Vector3.zero; });
            }));

            fieldPlacementScale.labelElement?.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
            {
                evt.menu.AppendAction("Reset", (e) => { fieldPlacementScale.value = Vector3.one; });
            }));

            fieldEraserSize.labelElement?.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
            {
                evt.menu.AppendAction("Reset", (e) => { fieldEraserSize.value = Vector3Int.one; });
            }));
            fieldEraserSize.RegisterValueChangedCallback(evt =>
            {
                fieldEraserSize.SetValueWithoutNotify(new Vector3Int(
                    evt.newValue.x < 1 ? 1 : evt.newValue.x,
                    evt.newValue.y < 1 ? 1 : evt.newValue.y,
                    evt.newValue.z < 1 ? 1 : evt.newValue.z
                ));
            });

            toggleEraserUsePlacementOffsets.RegisterValueChangedCallback(evt => 
            {
                toggleEraserOffset.SetValueWithoutNotify(evt.newValue);
                toggleEraserRotationOffset.SetValueWithoutNotify(evt.newValue);
                toggleEraserScaleOffset.SetValueWithoutNotify(evt.newValue);
            });
            toggleEraserOffset.RegisterValueChangedCallback(evt => 
            {
                if (!evt.newValue)
                    toggleEraserUsePlacementOffsets.SetValueWithoutNotify(false);
            });
            toggleEraserRotationOffset.RegisterValueChangedCallback(evt =>
            {
                if (!evt.newValue)
                    toggleEraserUsePlacementOffsets.SetValueWithoutNotify(false);
            });
            toggleEraserScaleOffset.RegisterValueChangedCallback(evt =>
            {
                if (!evt.newValue)
                    toggleEraserUsePlacementOffsets.SetValueWithoutNotify(false);
            });

            btnSelectAllInLayer.clicked += SelectAllTilesInLayer;
            btnClearSelection.clicked += () => { Selection.objects = null; };

            ToolbarToggle[] toolbarSelectionModeToggles = toolbarSelectionMode.Children().Cast<ToolbarToggle>().ToArray();
            if (toolbarSelectionModeToggles != null && toolbarSelectionModeToggles.Length > 0)
            {
                foreach (ToolbarToggle toggle in toolbarSelectionModeToggles)
                {
                    string selectionModeStr = "";
                    if (toggle != null && toggle.text != null)
                        selectionModeStr = Regex.Match(toggle.text, regexValidEnum).Value;

                    ESelectionMode selectionMode = ESelectionMode.Default;
                    try
                    {
                        selectionMode = Enum.Parse<ESelectionMode>(selectionModeStr);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error parsing ESelectionMode enum from toggle text. " + e.Message);
                    }
                    toggle.userData = selectionMode;

                    toggle.RegisterValueChangedCallback((evt) =>
                    {
                        if (evt.newValue == evt.previousValue)
                            return;

                        SelectionMode = selectionMode;
                    });
                }

                SelectionMode = ESelectionMode.Default;
            }

            fieldWandFilter.Init(EWandFilter.SamePaletteTile);
            fieldWandFilter.RegisterValueChangedCallback(evt => 
            {
                if (evt.newValue == evt.previousValue)
                    return;

                WandFilter = (EWandFilter)evt.newValue;
            });

            btnCombineMeshes.clicked += CombineSelectedMeshes;

            btnPaletteTilePreviewRefresh.clicked += RefreshTilePreviewImages;

            sliderPaletteTileWidth.SetValueWithoutNotify(paletteView.imageWidthInPixels);
            sliderPaletteTileWidth.RegisterValueChangedCallback(evt => 
            {
                paletteView.imageWidthInPixels = evt.newValue;
            });

            sliderPaletteTileHeight.SetValueWithoutNotify(paletteView.imageHeightInPixels);
            sliderPaletteTileHeight.RegisterValueChangedCallback(evt =>
            {
                paletteView.imageHeightInPixels = evt.newValue;
            });

            fieldPaletteTileFilter.RegisterValueChangedCallback(evt => 
            {
                paletteView.FilterTiles(evt.newValue);
            });

            fieldPalette.objectType = typeof(TilePalette);
            fieldPalette.RegisterValueChangedCallback(evt => 
            {
                TilePalette palette = evt.newValue as TilePalette;
                paletteView.value = palette;
            });

            rootVisualElement.focusable = true;
            rootVisualElement.pickingMode = PickingMode.Position;

            LoadEditorPreferences();

            Focus();
            OnSelectionChanged();

            GlobalEventHandler.onGlobalEvent -= HandleGlobalEvents;
            GlobalEventHandler.onGlobalEvent += HandleGlobalEvents;

            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;

            SceneView.duringSceneGui -= DuringSceneGUI;
            SceneView.duringSceneGui += DuringSceneGUI;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            Tools.current = Tool.View;
        }
        #endregion

        protected void OnFocus()
        {
            rootVisualElement.Focus();
        }

        protected void OnGUI()
        {
            isDrawingIMGUI = true;
        }

        protected void OnLostFocus()
        {
            isDrawingIMGUI = false;
        }

        protected void OnDisable()
        {
            GlobalEventHandler.onGlobalEvent -= HandleGlobalEvents;
            Selection.selectionChanged -= OnSelectionChanged;
            SceneView.duringSceneGui -= DuringSceneGUI;

            // when maximizing and the unmaximizing the window, it seems that Unity creates a new window and moves the visual tree over to it.
            // as such, UI element queried references end up being null and this causes NullReferenceExceptions so we check to see if
            // the rootVisualElement contains any children before calling any functions that use UI element references
            if (rootVisualElement.childCount > 0)
            {
                SetTilemapGizmoVisibility(false);
                SaveEditorPreferences();
            }
        }

        protected void OnDestroy()
        {
            instance = null;
        }

        private void LoadEditorPreferences()
        {
            fieldGridPosY.value = EditorPrefs.GetInt(EDITOR_PREF_KEY_GRID_POS_Y, 0);

            string gridPlaneExtentsStr = EditorPrefs.GetString(EDITOR_PREF_KEY_GRID_PLANE_EXTENTS, null);
            if (!string.IsNullOrEmpty(gridPlaneExtentsStr))
                fieldGridGizmoExtents.value = Vector2Int.RoundToInt(JsonUtility.FromJson<Vector2>(gridPlaneExtentsStr));
            else
                fieldGridGizmoExtents.value = DEFAULT_GRID_GIZMO_EXTENTS;

            string gridColorStr = EditorPrefs.GetString(EDITOR_PREF_KEY_GRID_COLOR, null);
            if (!string.IsNullOrEmpty(gridColorStr))
                fieldGridGizmoColor.value = JsonUtility.FromJson<Color>(gridColorStr);
            else
                fieldGridGizmoColor.value = DEFAULT_GRID_GIZMO_COLOR;

            string placementGizmoColorStr = EditorPrefs.GetString(EDITOR_PREF_KEY_PLACEMENT_GIZMO_COLOR, null);
            if (!string.IsNullOrEmpty(placementGizmoColorStr))
                fieldPlacementGizmoColor.value = JsonUtility.FromJson<Color>(placementGizmoColorStr);
            else
                fieldPlacementGizmoColor.value = Color.cyan;

            toggleUnpackPrefab.value = EditorPrefs.GetBool(EDITOR_PREF_KEY_UNPACK_PREFAB, true);

            string fieldPlacementOffsetStr = EditorPrefs.GetString(EDITOR_PREF_KEY_PLACEMENT_OFFSET, null);
            if (!string.IsNullOrEmpty(fieldPlacementOffsetStr))
                fieldPlacementOffset.value = JsonUtility.FromJson<Vector3>(fieldPlacementOffsetStr);
            else
                fieldPlacementOffset.value = Vector3.zero;

            string fieldPlacementRotationStr = EditorPrefs.GetString(EDITOR_PREF_KEY_PLACEMENT_ROTATION, null);
            if (!string.IsNullOrEmpty(fieldPlacementRotationStr))
                fieldPlacementRotation.value = JsonUtility.FromJson<Vector3>(fieldPlacementRotationStr);
            else
                fieldPlacementRotation.value = Vector3.zero;

            string fieldPlacementScaleStr = EditorPrefs.GetString(EDITOR_PREF_KEY_PLACEMENT_SCALE, null);
            if (!string.IsNullOrEmpty(fieldPlacementScaleStr))
                fieldPlacementScale.value = JsonUtility.FromJson<Vector3>(fieldPlacementScaleStr);
            else
                fieldPlacementScale.value = Vector3.one;

            string eraserSizeStr = EditorPrefs.GetString(EDITOR_PREF_KEY_ERASER_SIZE, null);
            if (!string.IsNullOrEmpty(eraserSizeStr))
                fieldEraserSize.value = Vector3Int.RoundToInt(JsonUtility.FromJson<Vector3>(eraserSizeStr));
            else
                fieldEraserSize.value = Vector3Int.one;

            toggleEraserUsePlacementOffsets.value = EditorPrefs.GetBool(EDITOR_PREF_KEY_ERASER_USE_PLACEMENT_OFFSETS, true);
            toggleEraserOffset.value = EditorPrefs.GetBool(EDITOR_PREF_KEY_ERASER_OFFSET_FLAG, true);
            toggleEraserRotationOffset.value = EditorPrefs.GetBool(EDITOR_PREF_KEY_ERASER_ROTATION_OFFSET_FLAG, true);
            toggleEraserScaleOffset.value = EditorPrefs.GetBool(EDITOR_PREF_KEY_ERASER_SCALE_OFFSET_FLAG, true);

            SelectionMode = (ESelectionMode)EditorPrefs.GetInt(EDITOR_PREF_KEY_SELECTION_MODE, 0);

            fieldWandFilter.value = (EWandFilter)EditorPrefs.GetInt(EDITOR_PREF_KEY_WAND_FILTER, 2);

            toggleMCDestroyTiles.value = EditorPrefs.GetBool(EDITOR_PREF_KEY_MESH_COMBINER_DESTROY_TILES, false);

            string prefabPalettePath = EditorPrefs.GetString(EDITOR_PREF_KEY_PREFAB_PALETTE_PATH, null);
            if (!string.IsNullOrEmpty(prefabPalettePath))
                fieldPalette.value = AssetDatabase.LoadAssetAtPath(prefabPalettePath, typeof(TilePalette));
            else
                paletteView.value = null;
        }

        private void SaveEditorPreferences()
        {
            EditorPrefs.SetFloat(EDITOR_PREF_KEY_GRID_POS_Y, fieldGridPosY.value);
            EditorPrefs.SetString(EDITOR_PREF_KEY_GRID_PLANE_EXTENTS, JsonUtility.ToJson((Vector2)fieldGridGizmoExtents.value));
            EditorPrefs.SetString(EDITOR_PREF_KEY_GRID_COLOR, JsonUtility.ToJson(fieldGridGizmoColor.value));
            EditorPrefs.SetString(EDITOR_PREF_KEY_PLACEMENT_GIZMO_COLOR, JsonUtility.ToJson(fieldPlacementGizmoColor.value));
            EditorPrefs.SetBool(EDITOR_PREF_KEY_UNPACK_PREFAB, toggleUnpackPrefab.value);
            EditorPrefs.SetString(EDITOR_PREF_KEY_PLACEMENT_OFFSET, JsonUtility.ToJson(fieldPlacementOffset.value));
            EditorPrefs.SetString(EDITOR_PREF_KEY_PLACEMENT_ROTATION, JsonUtility.ToJson(fieldPlacementRotation.value));
            EditorPrefs.SetString(EDITOR_PREF_KEY_PLACEMENT_SCALE, JsonUtility.ToJson(fieldPlacementScale.value));
            EditorPrefs.SetString(EDITOR_PREF_KEY_ERASER_SIZE, JsonUtility.ToJson((Vector3)fieldEraserSize.value));
            EditorPrefs.SetBool(EDITOR_PREF_KEY_ERASER_USE_PLACEMENT_OFFSETS, toggleEraserUsePlacementOffsets.value);
            EditorPrefs.SetBool(EDITOR_PREF_KEY_ERASER_OFFSET_FLAG, toggleEraserOffset.value);
            EditorPrefs.SetBool(EDITOR_PREF_KEY_ERASER_ROTATION_OFFSET_FLAG, toggleEraserRotationOffset.value);
            EditorPrefs.SetBool(EDITOR_PREF_KEY_ERASER_SCALE_OFFSET_FLAG, toggleEraserScaleOffset.value);
            EditorPrefs.SetInt(EDITOR_PREF_KEY_SELECTION_MODE, (int)SelectionMode);
            EditorPrefs.SetInt(EDITOR_PREF_KEY_WAND_FILTER, (int)(EWandFilter)fieldWandFilter.value);
            EditorPrefs.SetBool(EDITOR_PREF_KEY_MESH_COMBINER_DESTROY_TILES, toggleMCDestroyTiles.value);
            EditorPrefs.SetString(EDITOR_PREF_KEY_PREFAB_PALETTE_PATH, AssetDatabase.GetAssetPath(fieldPalette.value));
        }

        private void OnSelectionChanged()
        {
            if (instance == null)
                return;

            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                if (selectedObject.scene == null || selectedObject.scene.name == null)
                    continue;

                // tilemap != null -> user clicked directly on tilemap
                Tilemap tilemap = selectedObject.GetComponent<Tilemap>();
                // tileLayer != null -> user clicked directly on tilelayer
                TileLayer tileLayer = selectedObject.GetComponent<TileLayer>();

                if (tileLayer != null)
                {
                    tilemap = selectedObject.GetComponentInParent<Tilemap>();
                }
                else if (tilemap != null && tilemap != fieldTilemap.value)
                {
                    tileLayer = selectedObject.GetComponentInChildren<TileLayer>();
                }
                else if (tilemap == null && tileLayer == null && fieldTilemap.value == null && fieldTileLayer.value == null)
                {
                    // if the user has selected an object that is not a tilemap or a tilelayer
                    // and there is no tilelayer and tilemap currently selected
                    // then look in the parents of the selected object for a tilelayer and select that
                    tileLayer = selectedObject.GetComponentInParent<TileLayer>();
                    tilemap = tileLayer == null ? null : tileLayer.GetComponentInParent<Tilemap>();
                }

                if (tilemap == null || tileLayer == null)
                    continue;

                if (tilemap != null && tilemap != fieldTilemap.value)
                    SetTilemapGizmoVisibility(false);

                fieldTilemap.value = tilemap;
                fieldTileLayer.value = tileLayer;

                UpdateTilemapGridGizmoProperties(false);
                SetTilemapGizmoVisibility(true);

                break;
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {
                fieldTilemap.value = null;
                fieldTileLayer.value = null;
                OnSelectionChanged();
            }
        }

        private void DuringSceneGUI(SceneView sceneView)
        {
            UpdateTargetPositions();

            if (isDrawingIMGUI)
                HandleSceneViewInput();

            if (shouldDrawSceneGizmos)
            {
                UpdateTilemapGridGizmoViewBias();
                DrawCellGuidanceGizmo();
                HandleUtility.Repaint();
            }
        }

        private void UpdateTargetPositions()
        {
            Vector2 mousePosition = Event.current.mousePosition;

            if (SceneView.currentDrawingSceneView == null)
                return;

            Camera sceneViewCam = SceneView.currentDrawingSceneView.camera;

            if (sceneViewCam == null)
                return;

            // if the user's mouse is outside the scene view then don't draw scene gizmos
            if ((mousePosition.x < 0 || mousePosition.x > sceneViewCam.pixelWidth) ||
               (mousePosition.y < 0 || mousePosition.y > sceneViewCam.pixelHeight))
            {
                shouldDrawSceneGizmos = false;
                return;
            }
            else
            {
                if (editMode != EEditMode.Default)
                    shouldDrawSceneGizmos = true;
                else
                    shouldDrawSceneGizmos = false;
            }

            if (fieldTilemap.value == null || fieldTileLayer.value == null)
                return;

            Tilemap tilemap = fieldTilemap.value as Tilemap;

            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            Plane horizontalPlane = new Plane(
                GetGridPlaneUp().normalized,
                tilemap.ConvertToVector3Position(new Vector3Int(0, fieldGridPosY.value, 0))
            );

            if (!horizontalPlane.Raycast(ray, out float enter))
                return;

            Vector3 hit = ray.GetPoint(enter);

            TargetGridPosition = tilemap.GetNearestGridCellPosition(hit);

            targetPosition = tilemap.ConvertToVector3Position(TargetGridPosition);
        }

        private Vector3 GetGridPlaneUp()
        {
            Vector3 gridPlaneUp = Vector3.up;

            if (fieldTilemap.value == null || fieldTileLayer.value == null)
                return gridPlaneUp;

            Tilemap tilemap = fieldTilemap.value as Tilemap;

            if (tilemap != null)
                gridPlaneUp = tilemap.transform.rotation * gridPlaneUp;

            return gridPlaneUp;
        }

        private void UpdateTilemapGridGizmoViewBias()
        {
            if (fieldTilemap.value == null || fieldTileLayer.value == null)
                return;

            Tilemap tilemap = fieldTilemap.value as Tilemap;
            tilemap.gizmoGridViewBias = GetGridPlaneUp() * 0.01f;
        }

        private void UpdateTilemapGridGizmoProperties(bool markDirty = true)
        {
            Tilemap tilemap = fieldTilemap.value as Tilemap;

            if (tilemap == null)
                return;

            tilemap.gizmoGridColor = fieldGridGizmoColor.value;
            tilemap.gizmoGridPositionY = tilemap.ConvertToVector3Position(new Vector3Int(0, fieldGridPosY.value, 0)).y;
            tilemap.gizmoGridSizeX = fieldGridGizmoExtents.value.x;
            tilemap.gizmoGridSizeY = fieldGridGizmoExtents.value.y;

            if (markDirty)
                EditorUtility.SetDirty(tilemap);
        }

        private void SetTilemapGizmoVisibility(bool isVisible)
        {
            if (fieldTilemap.value == null || fieldTileLayer.value == null)
                return;

            Tilemap tilemap = fieldTilemap.value as Tilemap;
            tilemap.shouldDrawGizmos = isVisible;
        }

        private void DrawCellGuidanceGizmo()
        {
            if (fieldTilemap.value == null || fieldTileLayer.value == null)
                return;

            Tilemap tilemap = fieldTilemap.value as Tilemap;
            Vector3 bias = tilemap.gizmoGridViewBias;
            Handles.zTest = CompareFunction.LessEqual;

            Color color = fieldPlacementGizmoColor.value;
            if (EditMode == EEditMode.Erase)
                color = Color.magenta;

            Vector3 drawPosition = Quaternion.Inverse(tilemap.transform.rotation) * targetPosition;

            CompareFunction originalZTest = Handles.zTest;

            Handles.zTest = CompareFunction.LessEqual;

            Color outlineColor = color;
            Color faceColor = color;
            faceColor.a *= 0.1f;

            Vector3Int gizmoSize = Vector3Int.one;
            Vector3 offset = fieldPlacementOffset.value;
            Quaternion rotationOffset = Quaternion.Euler(fieldPlacementRotation.value);
            Vector3 scaleOffset = fieldPlacementScale.value;

            if (EditMode == EEditMode.Erase)
            {
                gizmoSize = fieldEraserSize.value;

                if (!toggleEraserOffset.value)
                    offset = Vector3.zero;

                if (!toggleEraserRotationOffset.value)
                    rotationOffset = Quaternion.identity;

                if (!toggleEraserScaleOffset.value)
                    scaleOffset = Vector3.one;
            }

            Matrix4x4 originalMatrix = Handles.matrix;
            Handles.matrix = Matrix4x4.TRS(
                Handles.matrix.GetPosition(),
                tilemap.transform.rotation,
                Handles.matrix.lossyScale
            );

            List<Vector3[]> faces = ComputeCellGuidanceGizmoVertices(drawPosition, tilemap, gizmoSize, bias, offset, rotationOffset, scaleOffset);
            Handles.DrawSolidRectangleWithOutline(faces[0], faceColor, outlineColor);
            Handles.DrawSolidRectangleWithOutline(faces[1], faceColor, outlineColor);
            Handles.DrawSolidRectangleWithOutline(faces[2], faceColor, outlineColor);
            Handles.DrawSolidRectangleWithOutline(faces[3], faceColor, outlineColor);
            Handles.DrawSolidRectangleWithOutline(faces[4], faceColor, outlineColor);
            Handles.DrawSolidRectangleWithOutline(faces[5], faceColor, outlineColor);
            Handles.DrawSolidRectangleWithOutline(faces[6], faceColor, outlineColor);

            Handles.matrix = originalMatrix;
            Handles.zTest = originalZTest;
        }

        /// <returns>
        /// A list of vertex arrays that are used to draw the rectangular faces that make up the cell guidance gizmo. <br />
        /// Order of rectangular faces : (center, bottom, 4 sides, top) .
        /// </returns>
        private List<Vector3[]> ComputeCellGuidanceGizmoVertices(Vector3 drawPosition, Tilemap tilemap, Vector3Int gizmoSize, Vector3 bias, 
            Vector3 offset, Quaternion rotationOffset, Vector3 scaleOffset)
        {
            List<Vector3[]> faces = new List<Vector3[]>();

            Vector3 scaledCellSize = new Vector3(
                tilemap.CellSize.x * tilemap.transform.lossyScale.x,
                tilemap.CellSize.y * tilemap.transform.lossyScale.y,
                tilemap.CellSize.z * tilemap.transform.lossyScale.z
            );
            Vector3 scaledCellGap = new Vector3(
                tilemap.CellGap.x * tilemap.transform.lossyScale.x,
                tilemap.CellGap.y * tilemap.transform.lossyScale.y,
                tilemap.CellGap.z * tilemap.transform.lossyScale.z
            );

            // *n represents the distance between the center of each cell
            float xn = (scaledCellGap.x + scaledCellSize.x);
            float yn = (scaledCellGap.y + scaledCellSize.y);
            float zn = (scaledCellGap.z + scaledCellSize.z);

            // *n(neg/pos) represents the distance to the respective edge of a rectangle (current equations favor negative side)
            // these are used to draw the rectangular slices that make up the gizmo and to handle the case of odd size values
            float xnNegative = (gizmoSize.x * xn / 2) - (scaledCellGap.x / 2) + (gizmoSize.x % 2 == 0 ? xn / 2 : 0);
            float ynNegative = (gizmoSize.y * yn / 2) - (scaledCellGap.y / 2) + (gizmoSize.y % 2 == 0 ? yn / 2 : 0);
            float znNegative = (gizmoSize.z * zn / 2) - (scaledCellGap.z / 2) + (gizmoSize.z % 2 == 0 ? zn / 2 : 0);
            float xnPositive = (gizmoSize.x * xn / 2) - (scaledCellGap.x / 2) - (gizmoSize.x % 2 == 0 ? xn / 2 : 0);
            float ynPositive = (gizmoSize.y * yn / 2) - (scaledCellGap.y / 2) - (gizmoSize.y % 2 == 0 ? yn / 2 : 0);
            float znPositive = (gizmoSize.z * zn / 2) - (scaledCellGap.z / 2) - (gizmoSize.z % 2 == 0 ? zn / 2 : 0);

            float xnNegativeWithScaleOffset = xnNegative * scaleOffset.x;
            float ynNegativeWithScaleOffset = ynNegative * scaleOffset.y;
            float znNegativeWithScaleOffset = znNegative * scaleOffset.z;
            float xnPositiveWithScaleOffset = xnPositive * scaleOffset.x;
            float ynPositiveWithScaleOffset = ynPositive * scaleOffset.y;
            float znPositiveWithScaleOffset = znPositive * scaleOffset.z;

            // center slice
            Vector3[] face = new Vector3[4];
            face[0] = new Vector3(drawPosition.x - xnNegative, drawPosition.y, drawPosition.z - znNegative) + bias;
            face[1] = new Vector3(drawPosition.x - xnNegative, drawPosition.y, drawPosition.z + znPositive) + bias;
            face[2] = new Vector3(drawPosition.x + xnPositive, drawPosition.y, drawPosition.z + znPositive) + bias;
            face[3] = new Vector3(drawPosition.x + xnPositive, drawPosition.y, drawPosition.z - znNegative) + bias;
            faces.Add(face);

            // cube faces (bottom, 4 sides, top) ...
            face = new Vector3[4];
            face[0] = new Vector3(drawPosition.x - xnNegativeWithScaleOffset, drawPosition.y - ynNegativeWithScaleOffset, drawPosition.z - znNegativeWithScaleOffset);
            face[0] = drawPosition + rotationOffset * (face[0] - drawPosition) + offset + bias;
            face[1] = new Vector3(drawPosition.x - xnNegativeWithScaleOffset, drawPosition.y - ynNegativeWithScaleOffset, drawPosition.z + znPositiveWithScaleOffset);
            face[1] = drawPosition + rotationOffset * (face[1] - drawPosition) + offset + bias;
            face[2] = new Vector3(drawPosition.x + xnPositiveWithScaleOffset, drawPosition.y - ynNegativeWithScaleOffset, drawPosition.z + znPositiveWithScaleOffset);
            face[2] = drawPosition + rotationOffset * (face[2] - drawPosition) + offset + bias;
            face[3] = new Vector3(drawPosition.x + xnPositiveWithScaleOffset, drawPosition.y - ynNegativeWithScaleOffset, drawPosition.z - znNegativeWithScaleOffset);
            face[3] = drawPosition + rotationOffset * (face[3] - drawPosition) + offset + bias;
            faces.Add(face);

            face = new Vector3[4];
            face[0] = new Vector3(drawPosition.x - xnNegativeWithScaleOffset, drawPosition.y - ynNegativeWithScaleOffset, drawPosition.z - znNegativeWithScaleOffset);
            face[0] = drawPosition + rotationOffset * (face[0] - drawPosition) + offset + bias;
            face[1] = new Vector3(drawPosition.x - xnNegativeWithScaleOffset, drawPosition.y + ynPositiveWithScaleOffset, drawPosition.z - znNegativeWithScaleOffset);
            face[1] = drawPosition + rotationOffset * (face[1] - drawPosition) + offset + bias;
            face[2] = new Vector3(drawPosition.x + xnPositiveWithScaleOffset, drawPosition.y + ynPositiveWithScaleOffset, drawPosition.z - znNegativeWithScaleOffset);
            face[2] = drawPosition + rotationOffset * (face[2] - drawPosition) + offset + bias;
            face[3] = new Vector3(drawPosition.x + xnPositiveWithScaleOffset, drawPosition.y - ynNegativeWithScaleOffset, drawPosition.z - znNegativeWithScaleOffset);
            face[3] = drawPosition + rotationOffset * (face[3] - drawPosition) + offset + bias;
            faces.Add(face);

            face = new Vector3[4];
            face[0] = new Vector3(drawPosition.x - xnNegativeWithScaleOffset, drawPosition.y - ynNegativeWithScaleOffset, drawPosition.z + znPositiveWithScaleOffset);
            face[0] = drawPosition + rotationOffset * (face[0] - drawPosition) + offset + bias;
            face[1] = new Vector3(drawPosition.x - xnNegativeWithScaleOffset, drawPosition.y + ynPositiveWithScaleOffset, drawPosition.z + znPositiveWithScaleOffset);
            face[1] = drawPosition + rotationOffset * (face[1] - drawPosition) + offset + bias;
            face[2] = new Vector3(drawPosition.x + xnPositiveWithScaleOffset, drawPosition.y + ynPositiveWithScaleOffset, drawPosition.z + znPositiveWithScaleOffset);
            face[2] = drawPosition + rotationOffset * (face[2] - drawPosition) + offset + bias;
            face[3] = new Vector3(drawPosition.x + xnPositiveWithScaleOffset, drawPosition.y - ynNegativeWithScaleOffset, drawPosition.z + znPositiveWithScaleOffset);
            face[3] = drawPosition + rotationOffset * (face[3] - drawPosition) + offset + bias;
            faces.Add(face);

            face = new Vector3[4];
            face[0] = new Vector3(drawPosition.x - xnNegativeWithScaleOffset, drawPosition.y - ynNegativeWithScaleOffset, drawPosition.z - znNegativeWithScaleOffset);
            face[0] = drawPosition + rotationOffset * (face[0] - drawPosition) + offset + bias;
            face[1] = new Vector3(drawPosition.x - xnNegativeWithScaleOffset, drawPosition.y + ynPositiveWithScaleOffset, drawPosition.z - znNegativeWithScaleOffset);
            face[1] = drawPosition + rotationOffset * (face[1] - drawPosition) + offset + bias;
            face[2] = new Vector3(drawPosition.x - xnNegativeWithScaleOffset, drawPosition.y + ynPositiveWithScaleOffset, drawPosition.z + znPositiveWithScaleOffset);
            face[2] = drawPosition + rotationOffset * (face[2] - drawPosition) + offset + bias;
            face[3] = new Vector3(drawPosition.x - xnNegativeWithScaleOffset, drawPosition.y - ynNegativeWithScaleOffset, drawPosition.z + znPositiveWithScaleOffset);
            face[3] = drawPosition + rotationOffset * (face[3] - drawPosition) + offset + bias;
            faces.Add(face);

            face = new Vector3[4];
            face[0] = new Vector3(drawPosition.x + xnPositiveWithScaleOffset, drawPosition.y - ynNegativeWithScaleOffset, drawPosition.z - znNegativeWithScaleOffset);
            face[0] = drawPosition + rotationOffset * (face[0] - drawPosition) + offset + bias;
            face[1] = new Vector3(drawPosition.x + xnPositiveWithScaleOffset, drawPosition.y + ynPositiveWithScaleOffset, drawPosition.z - znNegativeWithScaleOffset);
            face[1] = drawPosition + rotationOffset * (face[1] - drawPosition) + offset + bias;
            face[2] = new Vector3(drawPosition.x + xnPositiveWithScaleOffset, drawPosition.y + ynPositiveWithScaleOffset, drawPosition.z + znPositiveWithScaleOffset);
            face[2] = drawPosition + rotationOffset * (face[2] - drawPosition) + offset + bias;
            face[3] = new Vector3(drawPosition.x + xnPositiveWithScaleOffset, drawPosition.y - ynNegativeWithScaleOffset, drawPosition.z + znPositiveWithScaleOffset);
            face[3] = drawPosition + rotationOffset * (face[3] - drawPosition) + offset + bias;
            faces.Add(face);

            face = new Vector3[4];
            face[0] = new Vector3(drawPosition.x - xnNegativeWithScaleOffset, drawPosition.y + ynPositiveWithScaleOffset, drawPosition.z - znNegativeWithScaleOffset);
            face[0] = drawPosition + rotationOffset * (face[0] - drawPosition) + offset + bias;
            face[1] = new Vector3(drawPosition.x - xnNegativeWithScaleOffset, drawPosition.y + ynPositiveWithScaleOffset, drawPosition.z + znPositiveWithScaleOffset);
            face[1] = drawPosition + rotationOffset * (face[1] - drawPosition) + offset + bias;
            face[2] = new Vector3(drawPosition.x + xnPositiveWithScaleOffset, drawPosition.y + ynPositiveWithScaleOffset, drawPosition.z + znPositiveWithScaleOffset);
            face[2] = drawPosition + rotationOffset * (face[2] - drawPosition) + offset + bias;
            face[3] = new Vector3(drawPosition.x + xnPositiveWithScaleOffset, drawPosition.y + ynPositiveWithScaleOffset, drawPosition.z - znNegativeWithScaleOffset);
            face[3] = drawPosition + rotationOffset * (face[3] - drawPosition) + offset + bias;
            faces.Add(face);

            return faces;
        }

        public void RefreshTilePreviewImages()
        {
            if (paletteView != null)
                paletteView.RefreshView();
        }

        private void AddTile()
        {
            AddTile(TargetGridPosition);
        }
        private void AddTile(Vector3Int gridCellPosition)
        {
            Tilemap tilemap = fieldTilemap.value as Tilemap;
            TileLayer tileLayer = fieldTileLayer.value as TileLayer;

            TilemapEditorUtility.AddTile(
                tilemap, tileLayer, gridCellPosition, paletteView.GetSelectedPaletteTileObject(),
                toggleUnpackPrefab.value, fieldPlacementOffset.value, fieldPlacementRotation.value, fieldPlacementScale.value
            );

            Selection.objects = null;
        }

        private void RemoveTiles()
        {
            Tilemap tilemap = fieldTilemap.value as Tilemap;
            TileLayer tileLayer = fieldTileLayer.value as TileLayer;

            TilemapEditorUtility.RemoveTiles(tilemap, tileLayer, TargetGridPosition, fieldEraserSize.value);
        }

        private void BucketTiles()
        {
            Tilemap tilemap = fieldTilemap.value as Tilemap;
            TileLayer tileLayer = fieldTileLayer.value as TileLayer;

            if (tilemap == null || tileLayer == null)
                return;

            Tile targetTile = tilemap.GetTile(TargetGridPosition, tileLayer);
            UnityEngine.Object targetTileObject = targetTile == null ? null : targetTile.gameObject;

            bool selectionContainsTargetTile = targetTileObject != null && Selection.Contains(targetTileObject);

            string bucketTileTypeString;
            string directionsString;
            if (targetTileObject == null)
            {
                bucketTileTypeString = "*empty* connecting";
                directionsString = " in all x and z directions from the selected cell.";
            }
            else if (selectionContainsTargetTile)
            {
                bucketTileTypeString = "*selected*";
                directionsString = ".";
            }
            else
            {
                bucketTileTypeString = "*similar* connecting";
                directionsString = " in all 26 directions from the selected cell.";
            }

            bool userConfirmed = EditorUtility.DisplayDialog(
                "Confirm Bucket Operation",
                "Note that Bucket Operation is irreversible at the moment. You selected an empty cell which can potentially fill the whole grid.\n\n" +
                "The bucket operation is an expensive operation which will search and fill all " +
                bucketTileTypeString + " cells" +
                directionsString + "\n\n" +
                "Do you wish to continue?",
                "Confirm",
                "Cancel"
            );

            if (!userConfirmed)
                return;

            if (targetTileObject == null)
            {
                foreach (Vector3Int emptyCell in BFSFindEmptyGridCells(tilemap, tileLayer, TargetGridPosition, fieldGridGizmoExtents.value))
                    AddTile(emptyCell);
            }
            else
            {
                UnityEngine.Object[] bucketObjects;

                if (selectionContainsTargetTile)
                    bucketObjects = Selection.gameObjects;
                else
                    bucketObjects = BFSFindSimilarTiles(tilemap, tileLayer, TargetGridPosition);

                if (bucketObjects == null)
                    return;

                for (int i = 0; i < bucketObjects.Length; i++)
                {
                    GameObject go = bucketObjects[i] as GameObject;
                    Tile tile = go == null ? null : go.GetComponent<Tile>();

                    if (tile != null)
                        AddTile(tile.GridCellPosition);
                }
            }
        }

        private void SelectTile(bool clearSelection = true, bool deselectPreviouslySelectedTiles = false)
        {
            Tilemap tilemap = fieldTilemap.value as Tilemap;
            TileLayer tileLayer = fieldTileLayer.value as TileLayer;

            if (tilemap == null || tileLayer == null) 
                return;

            UnityEngine.Object[] newlySelectedTileObjects = null;

            if (SelectionMode == ESelectionMode.Default)
            {
                // select single tile ...
                Tile targetTile = tilemap.GetTile(TargetGridPosition, tileLayer);
                
                if (targetTile == null)
                {
                    if (clearSelection)
                        Selection.objects = null;

                    return;
                }

                newlySelectedTileObjects = new UnityEngine.Object[] { targetTile.gameObject };
            }
            else if (SelectionMode == ESelectionMode.Wand)
            {
                // select multiple tiles ...
                newlySelectedTileObjects = BFSFindSimilarTiles(tilemap, tileLayer, TargetGridPosition);
            }

            if (clearSelection)
                Selection.objects = newlySelectedTileObjects;
            else
            {
                if (newlySelectedTileObjects != null && newlySelectedTileObjects.Length > 0)
                {
                    UnityEngine.Object[] prevObjects = Selection.objects;
                    int prevObjectsLength = prevObjects == null ? 0 : prevObjects.Length;

                    List<UnityEngine.Object> newSelection = new List<UnityEngine.Object>(prevObjectsLength + newlySelectedTileObjects.Length);

                    for (int i = 0; i < prevObjectsLength; i++)
                        newSelection.Add(prevObjects[i]);

                    for (int i = 0; i < newlySelectedTileObjects.Length; i++)
                        newSelection.Add(newlySelectedTileObjects[i]);

                    if (deselectPreviouslySelectedTiles)
                        newSelection = newSelection.Where(x => !(prevObjects.Contains(x) && newlySelectedTileObjects.Contains(x))).ToList();

                    Selection.objects = newSelection.ToArray();
                }
            }
        }

        private void DeselectObject(UnityEngine.Object obj)
        {
            List<UnityEngine.Object> objList = Selection.objects.ToList();
            objList.Remove(obj);
            Selection.objects = objList.ToArray();
        }

        private UnityEngine.Object[] BFSFindSimilarTiles(Tilemap tilemap, TileLayer tileLayer, Vector3Int startCell)
        {
            List<UnityEngine.Object> tileObjects = new List<UnityEngine.Object>();

            if (tilemap == null || tileLayer == null)
                return tileObjects.ToArray();

            PriorityQueue<int, Vector3Int> openlist = new PriorityQueue<int, Vector3Int>();
            openlist.Push(0, startCell);
            HashSet<Vector3Int> closedlist = new HashSet<Vector3Int> { startCell };

            Tile targetTile = null;
            RulesetTile targetRulesetTile = null;
            if (wandFilter != EWandFilter.Any)
            {
                if (tilemap.TryGetTile(startCell, tileLayer, out targetTile))
                {
                    if (targetTile.TryGetComponent(out RulesetTileBehavior rulesetTileBehavior))
                        targetRulesetTile = rulesetTileBehavior.RulesetTile;
                }
            }

            while (!openlist.Empty)
            {
                Vector3Int currentCell = openlist.Pop().Value;

                Tile tile = tilemap.GetTile(currentCell, tileLayer);
                if (tile == null)
                    continue;

                if (wandFilter == EWandFilter.SameRuleset || (wandFilter == EWandFilter.SamePaletteTile && targetRulesetTile != null))
                {
                    if (tile.TryGetComponent(out RulesetTileBehavior rulesetTileBehavior))
                    {
                        if (rulesetTileBehavior.RulesetTile != targetRulesetTile)
                            continue;
                    }
                    else if (targetRulesetTile != null)
                    {
                        // user selected a ruleset tile but this current tile is not a ruleset tile ...
                        continue;
                    }
                }
                else if (wandFilter == EWandFilter.SamePaletteTile && targetRulesetTile == null)
                {
                    if (targetTile != null && targetTile.SourcePrefab != tile.SourcePrefab)
                        continue;
                }

                tileObjects.Add(tile.gameObject); 

                Tile[] neighborTiles = Tile.GetNeighborTiles(tilemap, tileLayer, currentCell);

                foreach (Tile nextTile in neighborTiles)
                {
                    if (nextTile == null)
                        continue;

                    if (!closedlist.Contains(nextTile.GridCellPosition))
                    {
                        openlist.Push(0, nextTile.GridCellPosition);
                        closedlist.Add(nextTile.GridCellPosition);
                    }
                }
            }

            return tileObjects.ToArray();
        }

        private List<Vector3Int> BFSFindEmptyGridCells(Tilemap tilemap, TileLayer tileLayer, Vector3Int startCell, Vector2Int gridExtents)
        {
            List<Vector3Int> emptyGridCells = new List<Vector3Int>();

            if (tilemap == null || tileLayer == null)
                return emptyGridCells;

            if (tilemap.TryGetTile(startCell, tileLayer, out _))
                return emptyGridCells;

            PriorityQueue<int, Vector3Int> openlist = new PriorityQueue<int, Vector3Int>();
            openlist.Push(0, startCell);
            HashSet<Vector3Int> closedlist = new HashSet<Vector3Int> { startCell };

            Vector3Int[] neighborGridCellOffsets = new Vector3Int[] 
            { 
                Vector3Int.forward, Vector3Int.right, Vector3Int.back, Vector3Int.left,
                Vector3Int.forward + Vector3Int.left, Vector3Int.forward, Vector3Int.right,
                Vector3Int.back + Vector3Int.right, Vector3Int.back, Vector3Int.left
            };
            Vector3Int[] neighborCells = new Vector3Int[8];

            void UpdateNeighborCells(Vector3Int currentCell)
            {
                neighborCells[0] = currentCell + neighborGridCellOffsets[0];
                neighborCells[1] = currentCell + neighborGridCellOffsets[1];
                neighborCells[2] = currentCell + neighborGridCellOffsets[2];
                neighborCells[3] = currentCell + neighborGridCellOffsets[3];
                neighborCells[4] = currentCell + neighborGridCellOffsets[4];
                neighborCells[5] = currentCell + neighborGridCellOffsets[5];
                neighborCells[6] = currentCell + neighborGridCellOffsets[6];
                neighborCells[7] = currentCell + neighborGridCellOffsets[7];
            }
            UpdateNeighborCells(startCell);

            float negativeXExtent = -(gridExtents.x / 2);
            float negativeZExtent = -(gridExtents.y / 2);
            float positiveXExtent = gridExtents.x % 2 == 0 ? (gridExtents.x / 2) - 1 : gridExtents.x / 2;
            float positiveZExtent = gridExtents.y % 2 == 0 ? (gridExtents.y / 2) - 1 : gridExtents.y / 2;

            while (!openlist.Empty)
            {
                Vector3Int currentCell = openlist.Pop().Value;

                if (currentCell.x < negativeXExtent || currentCell.x > positiveXExtent ||
                    currentCell.z < negativeZExtent || currentCell.z > positiveZExtent)
                {
                    continue;
                }

                if (currentCell != startCell && tilemap.TryGetTile(currentCell, tileLayer, out _))
                    continue;

                emptyGridCells.Add(currentCell);

                UpdateNeighborCells(currentCell);

                foreach (Vector3Int nextCell in neighborCells)
                {
                    if (tilemap.TryGetTile(nextCell, tileLayer, out _))
                        continue;

                    if (!closedlist.Contains(nextCell))
                    {
                        openlist.Push(0, nextCell);
                        closedlist.Add(nextCell);
                    }
                }
            }

            return emptyGridCells;
        }

        private void SelectAllTilesInLayer()
        {
            TileLayer tileLayer = fieldTileLayer.value as TileLayer;

            if (tileLayer == null)
                return;

            Tile[] tiles = tileLayer.GetComponentsInChildren<Tile>();
            GameObject[] tileObjects = tiles.Select(t => t.gameObject).ToArray();
            Selection.objects = tileObjects;
        }

        private void CombineSelectedMeshes()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            Transform parent = null;
            if (fieldTileLayer.value != null)
                parent = ((TileLayer)fieldTileLayer.value).transform;

            if (parent == null)
                return;

            List<MeshFilter> meshFilters = new List<MeshFilter>();
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                if (!selectedObject.TryGetComponent<Tile>(out _))
                    continue;

                MeshFilter[] meshFilterChildren = selectedObject.GetComponentsInChildren<MeshFilter>();

                meshFilters.AddRange(meshFilterChildren);

                if (meshFilterChildren.Length > 0)
                    selectedObject.SetActive(false);
            }

            string name = $"MeshCombine - {DateTimeOffset.Now.ToUnixTimeMilliseconds()}";

            GameObject combinedObject = MeshCombiner.CombineMeshObjects(meshFilters.ToArray(), name, parent);

            if (toggleMCDestroyTiles.value)
            {
                List<RulesetTile> destroyedRulesetTileRefs = new List<RulesetTile>();
                foreach (GameObject selectedObject in Selection.gameObjects)
                {
                    if (!selectedObject.TryGetComponent<Tile>(out _))
                        continue;

                    if (selectedObject.TryGetComponent(out RulesetTileBehavior rulesetTileBehavior))
                        destroyedRulesetTileRefs.Add(rulesetTileBehavior.RulesetTile);

                    if (Application.isPlaying)
                        Destroy(selectedObject);
                    else
                        DestroyImmediate(selectedObject);
                }

                foreach (RulesetTile rulesetTile in destroyedRulesetTileRefs)
                {
                    if (rulesetTile != null)
                        rulesetTile.ValidateRulesetTilesInScene();
                }
            }

            if (combinedObject != null)
                Selection.objects = new UnityEngine.Object[] { combinedObject };

            EditorSceneManager.MarkSceneDirty(combinedObject.gameObject.scene);
        }

        private void HandleSceneViewInput()
        {
            Event evt = Event.current;

            if (evt == null)
                return;

            if (EditMode != EEditMode.Default && evt.type == EventType.Layout)
            {
                int controlId = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
                HandleUtility.AddDefaultControl(controlId);
            }

            if ((evt.type == EventType.MouseDown || evt.type == EventType.MouseDrag) && evt.button == 0 && editMode != EEditMode.Default)
            {
                bool mouseDownOnNewCell = !mouseDownGridCell.HasValue || (mouseDownGridCell.HasValue && mouseDownGridCell.Value != TargetGridPosition);
                mouseDownGridCell = TargetGridPosition;

                if (fieldTilemap.value != null && fieldTileLayer.value != null && mouseDownOnNewCell)
                {
                    if (editMode == EEditMode.Paint)
                    {
                        if (placementMode == EPlacementMode.Default)
                            AddTile();
                        else if (placementMode == EPlacementMode.Bucket)
                            BucketTiles();
                    }
                    else if (editMode == EEditMode.Erase)
                    {
                        RemoveTiles();
                    }
                    else if (editMode == EEditMode.Select)
                    {
                        Tilemap tilemap = fieldTilemap.value as Tilemap;
                        TileLayer tileLayer = fieldTileLayer.value as TileLayer;
                        if (tilemap != null && tileLayer != null)
                        {
                            Tile tile = tilemap.GetTile(TargetGridPosition, tileLayer);
                            UnityEngine.Object tileObject = tile == null ? null : tile.gameObject;
                            if ((evt.control && evt.shift && evt.type == EventType.MouseDrag) || 
                                (evt.control && evt.type == EventType.MouseDown && tileObject != null && Selection.Contains(tileObject)))
                                DeselectObject(tileObject);
                            else
                                SelectTile(!evt.control);
                        }
                    }
                }

                evt.Use();
            }
            else if (evt.type == EventType.MouseUp && evt.button == 0)
            {
                mouseDownGridCell = null;
            }
        }

        public void HandleGlobalEvents(Event evt)
        {
            if (evt == null || !isDrawingIMGUI)
                return;

            if (evt.type == EventType.KeyDown)
            {
                if (evt.control && evt.keyCode == KeyCode.S)
                {
                    SaveEditorPreferences();
                }
                else if (evt.keyCode == KeyCode.Alpha1)
                {
                    EditMode = EEditMode.Default;
                    evt.Use();
                }
                else if (evt.keyCode == KeyCode.Alpha2)
                {
                    EditMode = EEditMode.Paint;
                    evt.Use();
                }
                else if (evt.keyCode == KeyCode.Alpha3)
                {
                    EditMode = EEditMode.Erase;
                    evt.Use();
                }
                else if (evt.keyCode == KeyCode.Alpha4)
                {
                    EditMode = EEditMode.Select;
                    evt.Use();
                }
                else if (evt.keyCode == KeyCode.Space)
                {
                    fieldGridPosY.value++;
                    evt.Use();
                }
                else if (!evt.control && evt.keyCode == KeyCode.C)
                {
                    fieldGridPosY.value--;
                    evt.Use();
                }
            }
        }
        
        public EEditMode EditMode
        {
            get => editMode;
            set
            {
                editMode = value;

                if (toolbarEditMode != null)
                {
                    foreach (ToolbarToggle toggle in toolbarEditMode.Children().Cast<ToolbarToggle>())
                        toggle.SetValueWithoutNotify(toggle.userData.Equals(editMode));

                    vePlacementOptions.style.display = editMode == EEditMode.Paint ? DisplayStyle.Flex : DisplayStyle.None;
                    vePalette.style.display = editMode == EEditMode.Paint ? DisplayStyle.Flex : DisplayStyle.None;

                    veEraserOptions.style.display = editMode == EEditMode.Erase ? DisplayStyle.Flex : DisplayStyle.None;

                    veSelectionOptions.style.display = editMode == EEditMode.Select ? DisplayStyle.Flex : DisplayStyle.None;
                    veMeshCombiner.style.display = editMode == EEditMode.Select ? DisplayStyle.Flex : DisplayStyle.None;
                }
            }
        }

        public EPlacementMode PlacementMode
        {
            get => placementMode;
            set
            {
                placementMode = value;

                if (toolbarPaintTool != null)
                {
                    foreach (ToolbarToggle toggle in toolbarPaintTool.Children().Cast<ToolbarToggle>())
                        toggle.SetValueWithoutNotify(toggle.userData.Equals(placementMode));
                }
            }
        }

        public ESelectionMode SelectionMode
        {
            get => selectionMode;
            set
            {
                selectionMode = value;

                if (toolbarSelectionMode != null)
                {
                    foreach (ToolbarToggle toggle in toolbarSelectionMode.Children().Cast<ToolbarToggle>())
                        toggle.SetValueWithoutNotify(toggle.userData.Equals(selectionMode));

                    fieldWandFilter.style.display = selectionMode == ESelectionMode.Wand ? DisplayStyle.Flex : DisplayStyle.None;
                }
            }
        }

        public EWandFilter WandFilter
        {
            get => wandFilter;
            set
            {
                wandFilter = value;
            }
        }

        private Vector3Int TargetGridPosition
        {
            get => new Vector3Int(fieldGridPosX.value, fieldGridPosY.value, fieldGridPosZ.value);
            set
            {
                fieldGridPosX.value = value.x;
                fieldGridPosY.value = value.y;
                fieldGridPosZ.value = value.z;
            }
        }
    }
}
