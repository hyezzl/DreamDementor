
using System.Collections.Generic;


public enum SceneType
{ 
    TitleScene,
    TutorialScene,
    HappyScene,
    HappyOneScene,
    RestScene,
    SorrowScene,
    SorrowOneScene,
    ChaosScene,
    HorrorScene,
    EndingScene,
}

public enum Speaker
{ 
    Player,
    Mom,
    Enemy,  // 메인빌런
    Extra,  // 파라미터로 관리할지 아니면 수적으면 그냥 번호붙이고???
}


// (일러스트에 표시될) 등장인물 감정
public enum Emotion
{
    //None            = -1,
    //Pbasic          = 0,
    //Psad            = 1,
    //Pembrassed      = 2, 
    //Pblind          = 3,
    //Psadhappy       = 4,
    //Ebasic          = 5,
    //Esmile          = 6,
    //EchillingSmile  = 7,
    None,
    PlayerEmotion,
    MomEmotion,
    EnemyEmotion,
    Extra01Emotion,
    Extra02Emotion,
}

public enum PlayerEmotion
{ 
    None            = -1,
    Basic           = 0,
    Sad             = 1,
    Embrassed       = 2,
    Blind           = 3,
    Happy           = 4,
}

public enum MomEmotion
{ 
    None            = -1,
    Basic           = 0,
    Smile           = 1,
    ChillingSmile   = 2,
    Satisfied       = 3,
    Dissatisfied    = 4,
}

public enum EnemyEmotion
{ 
    None        = -1,
}

public enum ExtraEmotion
{ 
    None        = -1,
}


public enum DeathType
{ 
    CrashEnemy,
    BadChoice,
    TimeAttack,
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
    public string eventDetailID;
    public int logID;
    public string dialog;
    public int nextID;
    public Speaker speaker;
    public string speakerName;
    public string choiceID;
    public Textbox textbox;
    public int emotion;
    public ChoiceData choices;  //  하위 클래스
}

public class ChoiceData
{
    public string choiceID;
    public List<string> texts;
    public List<int> scores;
    public List<string> continueIDs;
}
