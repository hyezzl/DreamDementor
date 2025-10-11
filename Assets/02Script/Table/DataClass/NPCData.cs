using UnityEngine;

public class NPCData
{
    public string npcID;
    public string npcName;
}

public class NPCDialogData
{
    public string npcID;
    public int logID;
    public string dialog;
    public int nextID;
    public Speaker speaker;
    public string speakerName;
    public string choiceID;
    public Textbox textbox;
    public int emotion;
    public ChoiceData choices;
}

public class NPCReDialogData
{
    public string npcID;
    public string dialog;
    public Speaker speaker;
    public string speakerName;
    public Textbox textbox;
    public int emotion;
}

