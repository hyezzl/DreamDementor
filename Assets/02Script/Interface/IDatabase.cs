using System.Collections.Generic;

public interface IDatabase
{
    PickableData GetPickable(int itemID);

    InteractableData GetInteractable(int itemID);

    InspectableData GetInspectable(int itemID);

    ReadableData GetReadable(int itemID);

    List<NarrationData> GetNarration(string eventID);

    List<CutsceneData> GetCutscene(string eventID);

    List<ConversationData> GetConversation(string eventID);

    ChoiceData GetChoice(string eventID);
}