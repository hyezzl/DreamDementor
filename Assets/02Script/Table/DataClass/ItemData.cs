using UnityEngine;

public class PickableData
{
    public int itemID;
    public string itemName;
    public ItemType type;
    public string description;
    public string reply;
    public Sprite icon;
    public string pairID;
    public bool subsist;
}

public class EatableData
{
    public int itemID;
    public string itemName;
    public ItemType type;
    public string description;
    public string reply;
    public Sprite icon;
    public int mental;
}


public class InteractableData
{
    public int itemID;
    public string itemName;
    public ItemType type;
    public string monologue;
    public string activeMSG;
    public string rejectMSG;
    public string pairID;
    public int hnum;
}

public class InspectableData
{
    public int itemID;
    public string itemName;
    public ItemType type;
    public string monologue;
}

//public class ReadableData
//{
//    public int itemID;
//    public string itemName;
//    public ItemType type;
//    public string monologue;
//    public string narrative;
//    public string reply;
//}

public class NoteData
{
    public int itemID;
    public string itemName;
    public ItemType type;
    public string text;
    public string reply;
}
