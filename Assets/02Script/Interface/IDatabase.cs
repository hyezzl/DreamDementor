using System.Collections.Generic;

public interface IDatabase
{
    PickableData GetPickable(int itemID);

    InteractableData GetInteractable(int itemID);

    InspectableData GetInspectable(int itemID);

    ReadableData GetReadable(int itemID);

    EventData GetEventData(string eventID);

    List<NarrationData> GetNarration(string eventID);

    List<DialogData> GetDialog(string eventID);

    ChoiceData GetChoice(string eventID);
}