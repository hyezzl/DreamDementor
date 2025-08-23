using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Tilemap3D;

using UnityObject = UnityEngine.Object;

namespace Tilemap3DEditor
{
    public class PaletteView : ScrollView
    {
        private bool isInitialized;
        private TilePalette palette;
        private VisualElement selectedTilePreview;

        internal void Init(bool isUxmlInitialization = false)
        {
            contentContainer.style.flexDirection = FlexDirection.Row;
            contentContainer.style.flexWrap = Wrap.Wrap;
            contentContainer.style.overflow = Overflow.Visible;

            isInitialized = true;

            SetValueWithoutNotify(palette);
        }

        public void RefreshView()
        {
            PopulateList();
        }

        public void PopulateListFromAssetsInFolder(string assetFolderPath = "Assets/")
        {
            Clear();

            List<IPaletteTile> paletteTiles = new List<IPaletteTile>();

            assetFolderPath = assetFolderPath.Trim();
            if (!assetFolderPath.StartsWith("Assets/") && !assetFolderPath.StartsWith("Assets\\"))
                assetFolderPath = "Assets/" + assetFolderPath;

            AssetDatabase.FindAssets("t:Prefab t:ScriptableObject", new string[] { assetFolderPath }).ToList().ForEach((guid) =>
            {
                UnityObject uObject = AssetDatabase.LoadAssetAtPath<UnityObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (uObject != null && uObject is IPaletteTile paletteTile)
                {
                    paletteTiles.Add(paletteTile);
                }
            });

            for (int i = 0; i < paletteTiles.Count; i++)
                CreateTilePreviewGUI(paletteTiles[i]);
        }

        private void PopulateList()
        {
            Clear();

            if (palette == null)
            {
                if (scanAssetsOnNullPalette)
                    PopulateListFromAssetsInFolder();

                return;
            }

            List<IPaletteTile> paletteTiles = palette.GetTiles();
            if (paletteTiles == null || paletteTiles.Count == 0)
                return;

            for (int i = 0; i < paletteTiles.Count; i++)
                CreateTilePreviewGUI(paletteTiles[i]);
        }

        private void CreateTilePreviewGUI(IPaletteTile paletteTile)
        {
            UnityObject paletteTileAsset = paletteTile as UnityObject;
            string name = "";
            string path = "";
            if (paletteTileAsset != null)
            {
                name = paletteTileAsset.name;
                path = AssetDatabase.GetAssetPath(paletteTileAsset);
            }

            Type tileType = null;
            if (paletteTile is Tile)
                tileType = typeof(Tile);
            else if (paletteTile is RulesetTile)
                tileType = typeof(RulesetTile);
            else if (paletteTile is RandomizerTile)
                tileType = typeof(RandomizerTile);

            IPaletteTile.PrefabData prefabData = paletteTile.GetPrefabData();

            VisualElement tilePreviewContainer = new VisualElement
            {
                tooltip = $"Name: {name}\nPath: {path}",
                userData = new TilePreviewContainerContextData()
                {
                    gameObject = prefabData == null ? null : paletteTile.GetPrefabData().prefab,
                    paletteTileObject = paletteTileAsset,
                    tileType = tileType
                }
            };
            tilePreviewContainer.RegisterCallback<MouseDownEvent>(OnTilePreviewClicked);

            tilePreviewContainer.style.overflow = Overflow.Hidden;
            tilePreviewContainer.style.width = imageWidthInPixels;
            tilePreviewContainer.style.marginTop = 4;
            tilePreviewContainer.style.marginRight = 4;
            tilePreviewContainer.style.marginBottom = 4;
            tilePreviewContainer.style.marginLeft = 4;
            tilePreviewContainer.style.borderTopWidth = 1;
            tilePreviewContainer.style.borderRightWidth = 1;
            tilePreviewContainer.style.borderBottomWidth = 1;
            tilePreviewContainer.style.borderLeftWidth = 1;
            tilePreviewContainer.style.borderTopColor = tilePreviewBorderColor;
            tilePreviewContainer.style.borderRightColor = tilePreviewBorderColor;
            tilePreviewContainer.style.borderBottomColor = tilePreviewBorderColor;
            tilePreviewContainer.style.borderLeftColor = tilePreviewBorderColor;
            tilePreviewContainer.style.borderTopRightRadius = 2;
            tilePreviewContainer.style.borderBottomRightRadius = 2;
            tilePreviewContainer.style.borderBottomLeftRadius = 2;
            tilePreviewContainer.style.borderTopLeftRadius = 2;

            IMGUIContainer imguiPreviewImage = new IMGUIContainer(() => DrawTilePreviewImage(tilePreviewContainer));

            tilePreviewContainer.Add(imguiPreviewImage);

            Label tilePreviewLabel = new Label(name)
            {
                tooltip = $"Name: {name}\nPath: {path}",
                displayTooltipWhenElided = false
            };

            tilePreviewLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            tilePreviewLabel.style.overflow = Overflow.Hidden;
            tilePreviewLabel.style.textOverflow = TextOverflow.Ellipsis;
            tilePreviewLabel.style.borderTopWidth = 1;
            tilePreviewLabel.style.borderTopColor = tilePreviewBorderColor;
            tilePreviewLabel.style.paddingTop = 2;
            tilePreviewLabel.style.paddingBottom = 2;

            tilePreviewContainer.Add(tilePreviewLabel);

            if (tileType != typeof(Tile))
            {
                Label watermarkLabel = new Label();

                if (tileType == typeof(RulesetTile))
                    watermarkLabel.text = "RT";
                else if (tileType == typeof(RandomizerTile))
                    watermarkLabel.text = " ?";

                watermarkLabel.style.position = Position.Absolute;
                watermarkLabel.style.top = 3;
                watermarkLabel.style.left = 3;
                watermarkLabel.style.color = Color.white;
                watermarkLabel.style.backgroundColor = Color.black;
                watermarkLabel.style.borderTopWidth = 1;
                watermarkLabel.style.borderRightWidth = 1;
                watermarkLabel.style.borderBottomWidth = 1;
                watermarkLabel.style.borderLeftWidth = 1;
                watermarkLabel.style.borderTopColor = Color.white;
                watermarkLabel.style.borderRightColor = Color.white;
                watermarkLabel.style.borderBottomColor = Color.white;
                watermarkLabel.style.borderLeftColor = Color.white;
                watermarkLabel.style.borderTopRightRadius = 2;
                watermarkLabel.style.borderBottomRightRadius = 2;
                watermarkLabel.style.borderBottomLeftRadius = 2;
                watermarkLabel.style.borderTopLeftRadius = 2;

                tilePreviewContainer.Add(watermarkLabel);
            }

            contentContainer.Add(tilePreviewContainer);
        }

        public UnityObject GetSelectedPaletteTileObject()
        {
            if (selectedTilePreview == null)
                return null;

            TilePreviewContainerContextData previewContextData = selectedTilePreview.userData as TilePreviewContainerContextData;
            if (previewContextData == null || previewContextData.paletteTileObject == null)
                return null;

            return previewContextData.paletteTileObject;
        }

        public Type GetSelectedPaletteTileType()
        {
            if (selectedTilePreview == null)
                return null;

            TilePreviewContainerContextData previewContextData = selectedTilePreview.userData as TilePreviewContainerContextData;
            if (previewContextData == null || previewContextData.paletteTileObject == null)
                return null;

            return previewContextData.tileType;
        }

        public IPaletteTile.PrefabData GetSelectedPaletteTilePrefabData(TileContext placementContext = null)
        {
            if (selectedTilePreview == null)
                return null;

            TilePreviewContainerContextData previewContextData = selectedTilePreview.userData as TilePreviewContainerContextData;
            if (previewContextData == null || previewContextData.paletteTileObject == null)
                return null;

            IPaletteTile paletteTile = previewContextData.paletteTileObject as IPaletteTile;
            if (paletteTile == null)
                return null;

            return paletteTile.GetPrefabData(placementContext);
        }

        private void DrawTilePreviewImage(VisualElement tilePreviewContainer)
        {
            TilePreviewContainerContextData previewContextData = tilePreviewContainer.userData as TilePreviewContainerContextData;
            Texture2D texture = null;

            if (previewContextData != null && previewContextData.gameObject != null)
            {
                if (previewContextData.previewTexture == null)
                {
                    if (previewContextData.tempPreviewTexture == null && !AssetPreview.IsLoadingAssetPreview(previewContextData.gameObject.GetInstanceID()))
                    {
                        previewContextData.tempPreviewTexture = AssetPreview.GetAssetPreview(previewContextData.gameObject);
                    }
                    else if (previewContextData.tempPreviewTexture != null)
                    {
                        previewContextData.previewTexture = new Texture2D(previewContextData.tempPreviewTexture.width, previewContextData.tempPreviewTexture.height);
                        previewContextData.previewTexture.SetPixels(previewContextData.tempPreviewTexture.GetPixels());
                        previewContextData.previewTexture.Apply();
                    }

                    texture = previewContextData.tempPreviewTexture;
                }
                else
                    texture = previewContextData.previewTexture;
            }

            if (texture != null)
            {
                Rect imageRect = new Rect(2, 2, imageWidthInPixels - 6, imageHeightInPixels - 6);
                GUI.DrawTexture(imageRect, texture, ScaleMode.StretchToFill, true, 0);
            }
            GUILayout.Space(imageHeightInPixels - 2);
            tilePreviewContainer.style.width = imageWidthInPixels;
        }

        private void OnTilePreviewClicked(MouseDownEvent evt)
        {
            VisualElement currentTarget = evt.currentTarget as VisualElement;

            if (currentTarget != null && evt.button == 2)
            {
                TilePreviewContainerContextData previewContextData = currentTarget.userData as TilePreviewContainerContextData;

                if (previewContextData != null)
                {
                    EditorGUIUtility.PingObject(previewContextData.paletteTileObject);

                    if (evt.clickCount == 2)
                    {
                        if (previewContextData.paletteTileObject is ScriptableObject)
                            Selection.objects = new UnityObject[] { previewContextData.paletteTileObject };
                        else if (previewContextData.paletteTileObject is MonoBehaviour mb)
                            Selection.objects = new UnityObject[] { mb.gameObject };
                    }

                    return;
                }
            }
            else if (evt.button == 0)
            {
                parent.Focus();

                bool isSameTileClicked = selectedTilePreview == currentTarget;

                ClearTileSelection();

                if (isSameTileClicked || currentTarget == null)
                    return;

                currentTarget.style.borderTopColor = Color.cyan;
                currentTarget.style.borderRightColor = Color.cyan;
                currentTarget.style.borderBottomColor = Color.cyan;
                currentTarget.style.borderLeftColor = Color.cyan;

                selectedTilePreview = currentTarget;
            }
        }

        public void FilterTiles(string filterText)
        {
            for (int i = 0; i < contentContainer.childCount; i++)
            {
                VisualElement tilePreviewContainer = contentContainer.ElementAt(i);
                TilePreviewContainerContextData contextData = tilePreviewContainer.userData as TilePreviewContainerContextData;

                string name = "";
                if (contextData.paletteTileObject != null && contextData.paletteTileObject.name != null)
                    name = contextData.paletteTileObject.name.Trim().ToLower();

                if (name.Contains(filterText.ToLower()))
                    tilePreviewContainer.style.display = DisplayStyle.Flex;
                else
                    tilePreviewContainer.style.display = DisplayStyle.None;
            }
        }

        public void ClearTileSelection()
        {
            if (selectedTilePreview == null)
                return;

            selectedTilePreview.style.borderTopColor = tilePreviewBorderColor;
            selectedTilePreview.style.borderRightColor = tilePreviewBorderColor;
            selectedTilePreview.style.borderBottomColor = tilePreviewBorderColor;
            selectedTilePreview.style.borderLeftColor = tilePreviewBorderColor;

            selectedTilePreview = null;
        }

        public void SetValueWithoutNotify(TilePalette palette)
        {
            this.palette = palette;
        }

        public TilePalette value
        {
            get => palette;
            set
            {
                palette = value;

                if (isInitialized)
                    PopulateList();
            }
        }

        private class TilePreviewContainerContextData
        {
            public Texture2D tempPreviewTexture;
            public Texture2D previewTexture;
            public GameObject gameObject;
            public UnityObject paletteTileObject;
            public Type tileType;
        }

        #region UI Builder Uxml Related
        public new class UxmlFactory : UxmlFactory<PaletteView, UxmlTraits> { }

        public new class UxmlTraits : ScrollView.UxmlTraits
        {
            // For pure UXML attributes to work, UI Builder requires your element class to expose a { get; set; } C# property that has the same name
            // as the name you set in your Uxml*AttributeDescription, except instead of dashes, the C# property name needs to be using camelCasing
            UxmlBoolAttributeDescription _scanAssetsOnNullPalette = new UxmlBoolAttributeDescription
            {
                name = "scan-assets-on-null-palette",
                defaultValue = false
            };
            UxmlIntAttributeDescription _imageWidthInPixels = new UxmlIntAttributeDescription
            {
                name = "image-width-in-pixels",
                defaultValue = 76
            };
            UxmlIntAttributeDescription _imageHeightInPixels = new UxmlIntAttributeDescription
            {
                name = "image-height-in-pixels",
                defaultValue = 76
            };
            UxmlColorAttributeDescription _tilePreviewBorderColor = new UxmlColorAttributeDescription
            {
                name = "tile-preview-border-color",
                defaultValue = new Color(0.129f, 0.129f, 0.129f, 1)
            };

            // describes what elements can be a child of this element.
            // uxmlChildElementsDescription is not enforced, it's only used to generate the uxml schema, which is used to help auto-completion in IDEs.
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get
                {
                    yield return new UxmlChildElementDescription(typeof(Label));
                    yield return new UxmlChildElementDescription(typeof(VisualElement));
                    yield break;
                }
            }

            public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext creationContext)
            {
                base.Init(element, bag, creationContext);

                PaletteView paletteView = element as PaletteView;

                paletteView.scanAssetsOnNullPalette = _scanAssetsOnNullPalette.GetValueFromBag(bag, creationContext);
                paletteView.imageWidthInPixels = _imageWidthInPixels.GetValueFromBag(bag, creationContext);
                paletteView.imageHeightInPixels = _imageHeightInPixels.GetValueFromBag(bag, creationContext);
                paletteView.tilePreviewBorderColor = _tilePreviewBorderColor.GetValueFromBag(bag, creationContext);

                paletteView.Init(true);
            }
        }

        // exposed UI Uxml properties
        public bool scanAssetsOnNullPalette { get; set; }
        public int imageWidthInPixels { get; set; }
        public int imageHeightInPixels { get; set; }
        public Color tilePreviewBorderColor { get; set; }
        #endregion
    }
}
