using System;


[Serializable]
public class NpcEntity 
{
    public string NpcID;
    public string NpcName;
}


[Serializable]
public class NpcDialogEntity
{
    public string NpcID;
    public string NpcEventID;
    public int LogID;
    public string Dialog;
    public int NextID;
    public string Speaker;
    public string SpeakerName;
    public string ChoiceID;
    public string Textbox;
    public int Emotion;
}


[Serializable]
public class NpcReDialogEntity
{
    public string NpcID;
    public string Dialog;
    public string Speaker;
    public string SpeakerName;
    public string Textbox;
    public int Emotion;
}
