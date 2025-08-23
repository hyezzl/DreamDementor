
- [User Documentation](#user-documentation)
  - [Assembly Info](#assembly-info)
  - [Creating Tilemap Objects](#creating-tilemap-objects)
  - [Using the Tilemap 3D Editor Window](#using-the-tilemap-3d-editor-window)

## User Documentation  

### Assembly Info
- Assembly Definition Files Path `{ASMDEF_PATH}` : `Assets/Tilemap3D/` 
  - Runtime : `{ASMDEF_PATH}/_Runtime/Tilemap3D.asmdef`
  - Editor : `{ASMDEF_PATH}/Editor/Tilemap3DEditor.asmdef`

In order to access the type and namespace identifiers contained in this package via script, you must ensure that the corresponding assembly definition file (asmdef) is properly referenced from your current assembly.

### Creating Tilemap Objects
Before starting to use the Tilemap 3D Editor Window, you must create a game object with a Tilemap component attached to it and a child object with a TileLayer component attached.  
Furthermore, in order to create Tile objects to use with the Tilemap Editor Window, you need to create one and also you need to create a Tile Palette asset which will contain references to your Tile objects.  

**Tilemap Object :**  
To create a game object with a Tilemap component attached to it, you can use the GameObject context menu (right click in hierarchy window) and select `Tilemap3D > Tilemap`. This will create a new Tilemap gameobject with a default TileLayer object as a child.  

**TileLayer Object :**  
To create a game object with a TileLayer component attached to it, you can use the GameObject context menu (right click in hierarchy window) and select `Tilemap3D > TileLayer`. This will create a new TileLayer object. If you are not creating the tile layer under an existing Tilemap object then this action will just create a new tilemap object with a default layer instead.  

**Tile Palette Asset :**  
A TilePalette is a container for tile prefabs.  
To create a TilePalette asset, simply right click in the project window and select `Create > Tilemap3D > TilePalette`. You can then add tiles to the list of tiles exposed in the TilePalette inspector.  
<a id="tile_palette_inspector"></a>
![Image of the TilePalette inspector](./assets/images/tile_palette_inspector.png "Image of the TilePalette inspector")

**Tile Prefab :**  
To add a Tile component to a game object, you can use the `Add Component` button in the inspector to add it. It will be located in the Add Component Menu under `Tilemap3D > Tile`.  
Once a game object has a Tile component attached, it is now considered a Tile object. Now, in order to use it with the tilemap editor window, you need to create a prefab out of it. Then, add the newly created prefab to a palette asset in the inspector.  

**RulesetTile Asset :**  
Ruleset tiles help to place tiles with respect to a set of rules. To create a ruleset tile asset, simply right click in the project window and select `Create > Tilemap3D > RulesetTile`. The prefab that will be used by the 3D tilemap system will be chosen based on the **first matching rule** in the list of rules.  

When inspecting a ruleset tile in the editor you should see something similar to the following:  
<a id="ruleset_tile_inspector"></a>
![Image of the RulesetTile inspector](./assets/images/ruleset_tile_inspector.png "Image of the RulesetTile inspector")

A ruleset tile has the following properties exposed in the inspector:  
- `Default Tile` : The default tile to use if no rules match.  
- `Rules` list : The list of rules to test against when determining which tile prefab to place. Each rule has the following properties:  
  - `Tile` : The tile associated with this particular rule.  
  - The match grid : an array of match types to be used to determine whether or not this rule matches the current placement context (all elements of the match grid must match in order for a rule to match). The grid is split into 3 smaller 9x9 grids that are used to represent the neighbors of a tile being placed. Try to picture the tile that will be placed as being in the "center" shown by the empty space in the middle of the second grid. Therefore, the first grid represents the "bottom" neighbors (y - 1), the second represents the "middle" neighbors (y = 0) and the third grid represents the "top" neighbors (y + 1). Each element in the grid can have one of the following 3 values:  
    - `[ ]` Anything : Specifies that this neighbor could be anything or nothing.  
    - `[✘]` Empty : Specifies that this neighbor must be empty.  
    - `[✔]` Occupied : Specifies that this neighbor must be occupied.  
  - `Transformation` : Used to transform the rule to try to reduce the amount of rules created by the user. The placed prefab will have the appropriate matched rotation/scale applied. The following options are available:  
    - `Fixed` : Do not apply any transformation to the match grid.  
    - `Rotate` X/Y/Z : Rotates the match grid clockwise around a specific axis 4 times in 90 degree increments, stops at the first match.  
    - `Mirror` X/Y/Z/XZ/XY/YZ : If the fixed match fails, then this flips the match grid on a specific axis and tries to match again.  
- `Revalidate Ruleset Tiles In Scene` button : Pressing this button will trigger an evaluation of all tiles in the scene that were placed and originated from this ruleset tile asset. This is useful if you modify this asset and need to update the tiles that you already placed in the scene.  

**RandomizerTile Asset :**  
Randomizer tiles are fairly straightforward, they randomly select a tile prefab from a given list. To create a randomizer tile asset, simply right click in the project window and select `Create > Tilemap3D > RandomizerTile`. The prefab that will be used by the 3D tilemap system will be chosen randomly from the list shown in the inspector.  

When inspecting a randomizer tile in the editor you should see something similar to the following:  
<a id="randomizer_tile_inspector"></a>
![Image of the RandomizerTile inspector](./assets/images/randomizer_tile_inspector.png "Image of the RandomizerTile inspector")

A randomizer tile has the following properties exposed in the inspector:  
- `Tiles` list : A list of tile prefabs to randomly choose from.  
- `ReRandomize Tiles In Scene` button : Pressing this button will re-randomize tiles in the scene that originated from this asset.  

### Using the Tilemap 3D Editor Window  
To open the Tilemap Editor Window navigate to the Unity Editor's toolbar and select `Tools > Tilemap3D > Tilemap3D Editor`. Only one instance of the editor window can be open at any given time. Also, the editor window is intended to be used alongside the scene view and hierarchy windows, so make sure you have those open as well.  
Note that, while the Tilemap Editor Window is open and not in `Select` mode, certain UI events in the scene view will be consumed and might have different results (like using transform gizmos, clicking and dragging, etc).

When the window is open you should see something similar to the following :  
<a id="tilemap_editor_window"></a>
![Image of the Tilemap editor window](./assets/images/tilemap_editor_window.png "Image of the Tilemap editor window")  

There are different mode toggles located at the top of the window (<span style="color:red;">red</span> part in above image). These modes will change the window's UI based on what tab is currently selected (<span style="color:green;">green</span> part in above image). There are however a few properties that remain "static" and are always shown (<span style="color:yellow;">yellow</span> part in above image), these are common properties that are used across multiple modes.  

In order to start using the editor window, select a gameobject in the hierarchy window that has a Tilemap or TileLayer component attached to it.  

**Common Properties :**  
<a id="tilemap_editor_window_common_props"></a>
![Image of the Tilemap editor window common properties](./assets/images/tilemap_editor_window_common_props.png "Image of the Tilemap editor window common properties")  
These properties are always shown regardless of what mode you are in.
- `Tile Map` : The last selected Tilemap object that we were editing. Must be selected in the hierarchy window.  
- `Tile Layer` : The last selected TileLayer object that we were editing. Must be selected in the hierarchy window.  
- `Grid Gizmo Color` : The color of the grid gizmo that is shown in the scene view window.  
- `Grid Gizmo Extents` : How far the grid gizmo should extend in the scene view window.  
- `Target Grid Position` : The grid position of the cell that the user current has their mouse over in the scene view. Note that only the `Y value` can be adjusted and it can be done by pressing the `spacebar` (increase) or the `C` key (decrease) while this editor window is open.  

**Default Mode :**  
<a id="tilemap_editor_window_default_options"></a>
![Image of the Tilemap editor window default options](./assets/images/tilemap_editor_window_default_options.png "Image of the Tilemap editor window default options")  
In this mode the user can use the scene view like they normally would without the Tilemap editor window consuming certain events (like transform gizmos, clicking and dragging, etc).  

**Paint Mode :**  
<a id="tilemap_editor_window_placement_options"></a>
![Image of the Tilemap editor window placement options](./assets/images/tilemap_editor_window_placement_options.png "Image of the Tilemap editor window placement options")  
In this mode, you can place gameobjects at exact cell positions in the grid by clicking and dragging in the scene view window while this editor window is open and in paint mode and there is a tile selected in the palette view.  

These options are only available if you select the `Paint` mode tab at the top of the editor window. You can also quickly switch to this mode by pressing the `alphanumeric number 2` key on your keyboard.
- `Tool` toggle : Switch between different placement tools (default, bucket, etc).  
  - `Default` tool : The default placement tool. Simply places prefab into the layer at the target grid position.
  - `Bucket` tool : Replaces all similar tiles with the selected tile from the palette. If you select tiles via selection mode first and then click within that selection, then all selected tiles will be replaced.
- `Unpack Prefab` : If checked, then the selected tile prefab will be completely unpacked when instantiated and placed into the scene.  
- `Placement Gizmo Color` : The color of the placement gizmo that is shown in the scene view that acts as a visual aid of where the tile prefab will be placed.  
- `Offset` : An offset applied to the position of the tile prefab when it is instantiated and placed into the scene.  
- `Rotation` : An offset applied to the rotation of the tile prefab when it is instantiated and placed into the scene.  
- `Scale` : A multiplier applied to the scale of the tile prefab when it is instantiated and placed into the scene.  
- `Palette` : The palette view shows all tiles within a given palette and allows the user to select a tile to use for placement. To begin using this view, make sure to select a TilePalette asset using the object field (very first field under the word "Palette")  
  - `Tile Previews` : 
    - `Refresh` button : Refreshes the image previews for all tiles in the current palette.  
    - `Width` slider : Changes the width of the image previews for all tiles in the current palette.  
    - `Height` slider : Changes the height of the image previews for all tiles in the current palette.  
    - `Filter` textbox : You can type in this textbox to filter the image previews by name for all tiles in the current palette.  
    - `Preview Boxes` : The bottom part of the Tile Previews section will have a list tile preview boxes from the current palette based on the filter. You can select/deselect a Tile to use by clicking on it's preview box. You can middle click on a preview box to ping the asset in the project view. You can also double middle click on a preview box to select the asset in the project view which will also display it in the Inspector view.  

**Erase Mode :**  
<a id="tilemap_editor_window_erase_options"></a>
![Image of the Tilemap editor window erase options](./assets/images/tilemap_editor_window_erase_options.png "Image of the Tilemap editor window erase options")  
In this mode, the user can click and drag to erase and destroy tile objects at exact grid locations in the current tile layer.  

These options are only available if you select the `Erase` mode tab at the top of the editor window. You can also quickly switch to this mode by pressing the `alphanumeric number 3` key on your keyboard.
- `Size` : The size of the eraser in grid cell units.  
- `Use Placement Offsets` : Whether or not the eraser gizmo should also use the placement offsets from the `Placement Options` view.  
  - `P` : Use Position offset.  
  - `R` : Use Rotation offset.  
  - `S` : Use Scale offset.  

**Select Mode :**  
<a id="tilemap_editor_window_select_options"></a>
![Image of the Tilemap editor window select options](./assets/images/tilemap_editor_window_select_options.png "Image of the Tilemap editor window select options")  
In this mode the user can select tile game objects in the current tile layer by clicking in the scene (clicking in an empty grid cell will deselect all).  

These options are only available if you select the `Select` mode tab at the top of the editor window. You can also quickly switch to this mode by pressing the `alphanumeric number 4` key on your keyboard.  

Selection shortcuts :  
`Left Click` => select tile.  
`Ctrl + Left Click` => select tile and append to current selection.  
`Ctrl + Shift + Left Click` => deselect tile and remove from current selection.  

- Current Layer buttons : Perform selection actions on the current tile layer.  
  - `Select All Tiles` button : Select all tiles in the current tile layer.  
  - `Clear Selection` button : Simply clears the current object selection.  
- `Tool` toggle : Switch between different selection tools (default, wand, etc).  
  - `Default` tool : The default selection tool. Simply selects the tile at the target grid position.
  - `Wand` tool : Selects a tile and all neighboring tiles that are similar to the selected tile.  
- `Wand Filter` dropdown : Select between different selection startegies for the wand tool.
  - `Any` : Select any tile that is a neighbor or neighbor of a neighbor of the selected tile.  
  - `Same Ruleset` : If the selected tile was a ruleset tile, then only select neighbors of the same ruleset tile type. If it was a non-ruleset tile, then only select neighbors that also do not originate from a ruleset tile.  
  - `Same Palette Tile` : If the selected tile was a ruleset tile, then the same logic is applied as for the "Same Ruleset" option. Otherwise, it was a non-ruleset tile, then only select neighbors that originated from the same tile prefab source.  

- **Mesh Combiner**
  - `Destroy Tiles?` toggle : Whether or not to destroy the tiles of the meshes used in the combining process.
  - `Combine Meshes` button : Pressing this button will combine the meshes of all selected game objects in the hierarchy window and will create a new game object with a mesh that is the combination of all these meshes. The newly created object is not a Tile and is not part of the tilemap, if you wish you can make it a tile by adding a Tile component to it and adding it to a palette. Note that the Tiles that were combined to make this new mesh object are simply deactivated so if you wish to remove them from the scene then you can do so by manually deleting them or using eraser mode.