using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

using System;

using Tilemap3D;

using UnityObject = UnityEngine.Object;

namespace Tilemap3DEditor
{
    [CustomPropertyDrawer(typeof(Rule))]
    public class RulePropertyDrawer : PropertyDrawer
    {
        private const string b64RotatedIcon = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAMAAAAoLQ9TAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAABCUExUReDg4Nra2o2NjUpKSisrK8vLy0ZGRiUlJUNDQ6ioqNnZ2dXV1ZKSkktLS3x8fIODgywsLKGhoSgoKLKysqWlpaSkpGPlOTEAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABsSURBVChTjY7LFoAgCETxOWpW2uP/fzXCOKddzYaZCyL0S8Y67501T6QQIYphZBORcik5IY4ZizTddUqwAhyy1JoxU2XjUQRggV/xAu1e3Njok74BW2ejS5lI1m93ouOQhh7GRDVOP5/0IaILsqoDYWCVRbEAAAAASUVORK5CYII=";
        private const string b64MirrorXIcon = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAMAAAAoLQ9TAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAABCUExUReDg4IODg3BwcC0tLQkJCcbGxrm5uc3Nzaamph4eHqGhocXFxQAAAKenp87OzigoKBYWFr29vb6+vtDQ0JGRkYGBgY6XDDwAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABZSURBVChTZcjZFkAwDEXRoEWpmf//VbkZunS5L8nZZGtae3xdsMdXQ+wBw2hJMU2AOZjElBWyCreDCLpADgut2xf2g0Qc0CIG2hAFb5YTcJXGGOr94H7kEL37XAQaUyWNygAAAABJRU5ErkJggg==";
        private const string b64MirrorZIcon = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAMAAAAoLQ9TAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAABCUExUReDg4M7OzsbGxigoKB4eHgAAAL6+vqGhobm5uZGRkS0tLYODg4GBgQkJCXBwcNDQ0MXFxc3Nzb29vRYWFqampqenp1RErOcAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABRSURBVChTVc1bDoAgDETRVsQ3Iqj736piJ7Fzf2BOSBCkHS5IQ0+iIUYvbXux7WQY0QTg5oVaZUvUjndcPlAB1NO+TRcA8m8Tv5vwfuX+DpEHzIoEOfv2YdIAAAAASUVORK5CYII=";
        private const string b64MirrorXZIcon = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAMAAAAoLQ9TAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAB+UExUReDg4IeHh5iYmIaGhgICAgUFBYqKigYGBgQEBAkJCZycnNzc3Nvb27y8vLW1tYmJidjY2C0tLdfX13Z2disrK97e3gAAALOzs3x8fKSkpAgICNnZ2Q8PD7Kysg4ODqOjozQ0NBISEtra2tPT087OztHR0aWlpZWVlaamppaWltuIZ3QAAAAJcEhZcwAADsMAAA7DAcdvqGQAAACCSURBVChTbYzbEsFAEEQHm8iIYBvjEsQ1+P8f1FlTqlLlPPT06YcRZzD04oxC1lvycVHoxKWjnJLS5Us180LmC0ZUBpbJAwRYKbCGccmDAZbY8G5ltzfUMWqIsYIdjsIFvx8nukhTM84XxjX5X2538nDpaNWseLokWs16LvJ6pyPyAX/IB7k2GqdnAAAAAElFTkSuQmCC";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty spTile = property.FindPropertyRelative(nameof(Rule.tile));
            SerializedProperty spMatchGrid = property.FindPropertyRelative(nameof(Rule.matchGrid));
            SerializedProperty spRuleTransformation = property.FindPropertyRelative(nameof(Rule.transformation));

            GroupBox boxRules = new GroupBox();
            boxRules.AddToClassList("rules");

            VisualElement veTileConfig = new VisualElement();
            veTileConfig.AddToClassList("rule-tileConfig");

            ObjectField fieldTile = new ObjectField("Tile")
            {
                allowSceneObjects = false,
                objectType = typeof(UnityObject)
            };
            fieldTile.BindProperty(spTile);
            fieldTile.AddToClassList("rule-tile");
            fieldTile.RegisterValueChangedCallback(evt =>
            {
                if (spTile.objectReferenceValue == null)
                    fieldTile.Q(null, "unity-object-field__input")?.AddToClassList("background-red");
                else
                    fieldTile.Q(null, "unity-object-field__input")?.RemoveFromClassList("background-red");
            });

            if (spTile.objectReferenceValue == null)
                fieldTile.Q(null, "unity-object-field__input")?.AddToClassList("background-red");
            else
                fieldTile.Q(null, "unity-object-field__input")?.RemoveFromClassList("background-red");

            veTileConfig.Add(fieldTile);

            IMGUIContainer imguiTilePreview = new IMGUIContainer();
            imguiTilePreview.onGUIHandler = () =>
            { 
                TilePreviewContainerContextData previewContextData = imguiTilePreview.userData as TilePreviewContainerContextData;
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
                    Rect imageRect = new Rect(3, 3, 64 - 6, 64 - 6);
                    GUI.DrawTexture(imageRect, texture, ScaleMode.StretchToFill, true, 0);
                }
            };
            imguiTilePreview.AddToClassList("rule-tilePreviewImage");

            fieldTile.RegisterValueChangedCallback(evt =>
            {
                Tile tile = evt.newValue as Tile;

                if (tile == null)
                {
                    GameObject go = evt.newValue as GameObject;
                    if (go != null)
                        tile = go.GetComponent<Tile>();
                }

                fieldTile.SetValueWithoutNotify(tile);

                imguiTilePreview.userData = new TilePreviewContainerContextData()
                {
                    gameObject = tile == null ? null : tile.gameObject,
                    previewTexture = null,
                    tempPreviewTexture = null
                };
            });

            if (spMatchGrid.arraySize != 27)
            {
                spMatchGrid.ClearArray();
                for (int i = 0; i < 27; i++)
                {
                    spMatchGrid.InsertArrayElementAtIndex(i);
                }
                spMatchGrid.serializedObject.ApplyModifiedProperties();
            }

            SerializedProperty spMatchGridCopy = spMatchGrid.Copy();
            void MatchGridOnClick(ClickEvent evt, int index)
            {
                if (index == 13)
                    return;

                int currentValue = spMatchGridCopy.GetArrayElementAtIndex(index).enumValueIndex;
                currentValue = currentValue >= 2 ? 0 : currentValue + 1;
                spMatchGridCopy.GetArrayElementAtIndex(index).enumValueIndex = currentValue;
                ((Button)evt.target).text = GetMatchTypeText((Rule.EMatchType)currentValue);
                spMatchGridCopy.serializedObject.ApplyModifiedProperties();

                evt.StopPropagation();
            }

            VisualElement veMatchGridBottom = new VisualElement();
            veMatchGridBottom.AddToClassList("rule-matchGrid");
            veMatchGridBottom.AddToClassList("bottom");

            for (var i = 0; i < 3; i++)
            {
                var row = new VisualElement();
                row.AddToClassList("rule-matchGrid-row");

                for (var j = 0; j < 3; j++)
                {
                    int index = i * 3 + j;
                    Button cell = new Button()
                    {
                        name = $"MatchGrid_{i}_{j}",
                        text = GetMatchTypeText((Rule.EMatchType)spMatchGrid.GetArrayElementAtIndex(index).enumValueIndex)
                    };
                    cell.AddToClassList("rule-matchGrid-cell");

                    cell.RegisterCallback<ClickEvent, int>(MatchGridOnClick, index);
                    row.Add(cell);
                }
                veMatchGridBottom.Add(row);
            }

            VisualElement veMatchGridMiddle = new VisualElement();
            veMatchGridMiddle.AddToClassList("rule-matchGrid");
            veMatchGridMiddle.AddToClassList("middle"); 

            for (var i = 0; i < 3; i++)
            {
                var row = new VisualElement();
                row.AddToClassList("rule-matchGrid-row");

                for (var j = 0; j < 3; j++)
                {
                    var cell = new VisualElement();
                    int index = 9 + (i * 3 + j);
                    if (i != 1 || j != 1)
                    {
                        cell = new Button()
                        {
                            name = $"MatchGrid_{i}_{j}",
                            text = GetMatchTypeText((Rule.EMatchType)spMatchGrid.GetArrayElementAtIndex(index).enumValueIndex)
                        };
                    }
                    cell.AddToClassList("rule-matchGrid-cell");

                    cell.RegisterCallback<ClickEvent, int>(MatchGridOnClick, index);
                    row.Add(cell);
                }
                veMatchGridMiddle.Add(row);
            }

            VisualElement veMatchGridTop = new VisualElement();
            veMatchGridTop.AddToClassList("rule-matchGrid");
            veMatchGridTop.AddToClassList("top");

            for (var i = 0; i < 3; i++)
            {
                var row = new VisualElement();
                row.AddToClassList("rule-matchGrid-row");

                for (var j = 0; j < 3; j++)
                {
                    int index = 18 + (i * 3 + j);
                    Button cell = new Button()
                    {
                        name = $"MatchGrid_{i}_{j}",
                        text = GetMatchTypeText((Rule.EMatchType)spMatchGrid.GetArrayElementAtIndex(index).enumValueIndex)
                    };
                    cell.AddToClassList("rule-matchGrid-cell");

                    cell.RegisterCallback<ClickEvent, int>(MatchGridOnClick, index);
                    row.Add(cell);
                }
                veMatchGridTop.Add(row);
            }

            VisualElement veTileConfigRow = new VisualElement();
            veTileConfigRow.AddToClassList("flex-direction-row");

            veTileConfigRow.Add(imguiTilePreview);
            veTileConfigRow.Add(veMatchGridBottom);
            veTileConfigRow.Add(veMatchGridMiddle);
            veTileConfigRow.Add(veMatchGridTop);

            veTileConfig.Add(veTileConfigRow);

            Image imgRuleTransformation = new Image();
            imgRuleTransformation.style.width = 16;
            imgRuleTransformation.style.height = 16;
            imgRuleTransformation.style.marginTop = 1;
            imgRuleTransformation.style.marginLeft = 5;
            imgRuleTransformation.AddToClassList("rule-transformation-img");

            EnumField fieldRuleTransformation = new EnumField("Transformation", (Rule.ETransformation)spRuleTransformation.enumValueIndex);
            fieldRuleTransformation.BindProperty(spRuleTransformation.Copy()); 
            fieldRuleTransformation.RegisterValueChangedCallback(evt => 
            {
                if (evt.newValue == null)
                    return;

                SetRuleTransformationTexture(imgRuleTransformation, (Rule.ETransformation)evt.newValue);
            });
            fieldRuleTransformation.AddToClassList("rule-transformation");
            fieldRuleTransformation.Q<VisualElement>(null, "unity-enum-field__input")?.Add(imgRuleTransformation);
            SetRuleTransformationTexture(imgRuleTransformation, (Rule.ETransformation)spRuleTransformation.enumValueIndex);

            veTileConfig.Add(fieldRuleTransformation);

            boxRules.Add(veTileConfig);

            return boxRules;
        }

        private void SetRuleTransformationTexture(Image imgRuleTransformation, Rule.ETransformation transformation)
        {
            if (transformation == Rule.ETransformation.RotateY || transformation == Rule.ETransformation.RotateX || transformation == Rule.ETransformation.RotateZ)
                imgRuleTransformation.image = Base64ToTexture(b64RotatedIcon);
            else if (transformation == Rule.ETransformation.MirrorX)
                imgRuleTransformation.image = Base64ToTexture(b64MirrorXIcon);
            else if (transformation == Rule.ETransformation.MirrorZ || transformation == Rule.ETransformation.MirrorY)
                imgRuleTransformation.image = Base64ToTexture(b64MirrorZIcon);
            else if (transformation == Rule.ETransformation.MirrorXZ || transformation == Rule.ETransformation.MirrorXY || transformation == Rule.ETransformation.MirrorYZ)
                imgRuleTransformation.image = Base64ToTexture(b64MirrorXZIcon);
            else
                imgRuleTransformation.image = null;

            imgRuleTransformation.style.display = imgRuleTransformation.image == null ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private static Texture2D Base64ToTexture(string base64)
        {
            Texture2D t = new Texture2D(1, 1);
            t.hideFlags = HideFlags.HideAndDontSave;
            t.LoadImage(Convert.FromBase64String(base64));
            return t;
        }

        private static string GetMatchTypeText(Rule.EMatchType value) => value switch
        {
            Rule.EMatchType.Anything => "",
            Rule.EMatchType.Empty => "✘",
            Rule.EMatchType.Occupied => "✔",
            _ => ""
        };

        private class TilePreviewContainerContextData
        {
            public Texture2D tempPreviewTexture;
            public Texture2D previewTexture;
            public GameObject gameObject;
        }
    }
}
