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

    Dictionary<string, Dictionary<int, DialogData>> GetDialogEvent(string eventID);

    Dictionary<int, DialogData> GetDialog(string eventID, string eventDetailID);

    ChoiceData GetChoice(string eventID);

    Dictionary<string, Dictionary<int, NPCDialogData>> GetNpcEvent(string npcID);

    Dictionary<int, NPCDialogData> GetNpcDialog(string npcID, string npcEventID);

    NPCReDialogData GetNpcReDialog(string npcID);
}