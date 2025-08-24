using System;

[Serializable]
public class NarrationEntity
{
    public string EventID;
    public string EventName;
    public string EventType;
    public int Order;
    public string Text;
}

[Serializable]
public class CutsceneEntity
{
    public string EventID;
    public string EventName;
    public string EventType;
    public int ConvID;
    public string Text;
    public int NextConvID;
    public string Speaker;
}

[Serializable]
public class ConversationEntity
{
    public string EventID;
    public string EventName;
    public string EventType;
    public int ConvID;
    public string Text;
    public int NextConvID;
    public string Speaker;
    public string ChoiceID;
    public int Emotion;
}

[Serializable]
public class ChoiceEntity
{
    public string ChoiceID;
    public string Choice0;
    public string Choice1;
    public string Choice2;
}
