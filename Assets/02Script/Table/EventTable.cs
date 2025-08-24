using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "02Script/Table/SO")]
public class EventTable : ScriptableObject
{
	public List<NarrationEntity> Narration; // Replace 'EntityType' to an actual type that is serializable.
	public List<CutsceneEntity> Cutscene; // Replace 'EntityType' to an actual type that is serializable.
	public List<ConversationEntity> Conversation; // Replace 'EntityType' to an actual type that is serializable.
	public List<ChoiceEntity> Choice; // Replace 'EntityType' to an actual type that is serializable.
}
