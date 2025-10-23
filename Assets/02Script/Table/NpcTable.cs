using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "02Script/Table/SO")]
public class NpcTable : ScriptableObject
{
	public List<NpcDialogEntity> NPC; // Replace 'EntityType' to an actual type that is serializable.
	public List<NpcDialogEntity> NpcDialog; // Replace 'EntityType' to an actual type that is serializable.
	public List<NpcReDialogEntity> NpcReDialog; // Replace 'EntityType' to an actual type that is serializable.
}
