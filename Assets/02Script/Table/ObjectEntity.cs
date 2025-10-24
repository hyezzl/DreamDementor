
using System;

[Serializable]
public class PickableEntity
{
    public int ItemID;
    public string ItemName;
    public string Type;
    public string Description;
    public string Reply;
    public string IconName;
    public string PairID;
    public int Subsist;
}

[Serializable]
public class EatableEntity
{ 
    public int ItemID;
    public string ItemName;
    public string Type;
    public string Description;
    public string Reply;
    public string IconName;
    public int Mental;
}

[Serializable]
public class InteractableEntity
{
    public int ItemID;
    public string ItemName;
    public string Type;
    public string Monologue;
    public string ActiveMSG;
    public string RejectMSG;
    public string PairID;
    public int HNum;
}

[Serializable]
public class InspectableEntity
{
    public int ItemID;
    public string ItemName;
    public string Type;
    public string Monologue;
}

//[Serializable]
//public class ReadableEntity
//{
//    public int ItemID;
//    public string ItemName;
//    public string Type;
//    public string Monologue;
//    public string Narrative;
//    public string Reply;
//}

[Serializable]
public class NoteEntity
{
    public int ItemID;
    public string ItemName;
    public string Type;
    public string Text;
    public string Reply;
}