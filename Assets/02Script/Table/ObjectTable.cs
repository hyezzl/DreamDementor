using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "02Script/Table/SO")]
public class ObjectTable : ScriptableObject
{
	public List<PickableEntity> Pickable; // Replace 'EntityType' to an actual type that is serializable.
	public List<EatableEntity> Eatable; // Replace 'EntityType' to an actual type that is serializable.
	public List<InteractableEntity> Interactable; // Replace 'EntityType' to an actual type that is serializable.
	public List<InspectableEntity> Inspectable; // Replace 'EntityType' to an actual type that is serializable.
}
