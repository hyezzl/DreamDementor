using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "02Script/Table/SO")]
public class EventTable : ScriptableObject
{
	public List<EventEntity> Event; // Replace 'EntityType' to an actual type that is serializable.
	public List<NarrationEntity> Narration; // Replace 'EntityType' to an actual type that is serializable.
	public List<DialogEntity> Dialog; // Replace 'EntityType' to an actual type that is serializable.
	public List<ChoiceEntity> Choice; // Replace 'EntityType' to an actual type that is serializable.
}
