using System;

[Serializable]
public class EventEntity
{
    public string EventID;
    public string EventName;
    public string EventType;
}

[Serializable]
public class NarrationEntity
{ 
    public string EventID;
    public int Order;
    public string Text;
}

[Serializable]
public class DialogEntity
{
    public string EventID;
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
public class ChoiceEntity
{
    public string ChoiceID;
    public string Choice0;
    public string Choice1;
    public string Choice2;
    public int Score0;
    public int Score1;
    public int Score2;
    public string Continue0;
    public string Continue1;
    public string Continue2;
}
