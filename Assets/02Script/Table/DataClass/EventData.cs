
using System.Collections.Generic;

public enum Speaker
{ 
    Player,
    Enemy,  // 메인빌런
    Extra,  // 파라미터로 관리할지 아니면 수적으면 그냥 번호붙이고???
}

public enum Emotion
{
    Pbasic = 0, 
    Psad = 1,
    Pembrassed = 2,
    Pblind = 3,
    PHappy = 4,
    Ebasic = 5,
    Echill = 6,
    //
}




public class EventData
{
    public string eventID;
    public string eventName;
    public EventType type;
}

public class NarrationData
{ 
    public string eventID;
    public int order;
    public string text;
}

public class DialogData
{
    public string eventID;
    public int logID;
    public string dialog;
    public int nextID;
    public Speaker speaker;
    public string speakerName;
    public string choiceID;
    public Emotion emotion;
    public ChoiceData choices;  //  하위 클래스
}

public class ChoiceData
{
    public string choiceID;
    public List<string> texts;
}
