
using System.Collections.Generic;

public enum Speaker
{ 
    Player,
    Enemy,  // 메인빌런
    Extra,  // 파라미터로 관리할지 아니면 수적으면 그냥 번호붙이고???
}


// (일러스트에 표시될) 등장인물 감정
public enum Emotion
{
    None            = -1,
    Pbasic          = 0,
    Psad            = 1,
    Pembrassed      = 2, 
    Pblind          = 3,
    Psadhappy       = 4,
    Ebasic          = 5,
    Esmile          = 6,
    EchillingSmile  = 7,
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
