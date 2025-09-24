using System.Collections.Generic;

public interface IDatabase
{
    PickableData GetPickable(int itemID);

    EatableData GetEatable(int itemID);

    InteractableData GetInteractable(int itemID);

    InspectableData GetInspectable(int itemID);

    //ReadableData GetReadable(int itemID);

    EventData GetEventData(string eventID);

    List<NarrationData> GetNarration(string eventID);

    Dictionary<int, DialogData> GetDialog(string eventID);

    ChoiceData GetChoice(string eventID);

    Dictionary<int, NPCDialogData> GetNpcDialog(string npcID);

    NPCReDialogData GetNpcReDialog(string npcID);
}