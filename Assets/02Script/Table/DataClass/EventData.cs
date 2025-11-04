
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
    ChaosOneScene,
    HorrorScene,
    HorrorOneScene,
    EndingScene,

    ClassroomScene,
    HomeScene,
    None,
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
    None,
    PlayerEmotion,
    MomEmotion,
    EnemyEmotion,
    Extra01Emotion,
    Extra02Emotion,
}

public enum PlayerEmotion
{ 
    None                       = -1,
    Idle                       = 0,
    Sad                        = 1,
    Sad_noteardrop             = 2,
    Sad_smile                  = 3,
    Sad_smile_noteardrop       = 4,
    Flustered_Idle             = 5,
    Gooseflesh_Idle            = 6,
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
    None                = -1,
    Somi_normal         = 0,
    Somi_cackle         = 1,
    Somi_smile          = 2,
    Somi_serious        = 3,
    Sena_normal         = 4,
    Sena_cackle         = 5,
    Sena_angry          = 6,
    Sena_serious        = 7,
    Hyuk_normal         = 8,
    Hyuk_cackle         = 9,
    Hyuk_smile          = 10,
    Hyuk_angry          = 11,
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
    public string rootID;
}

public class QuestData
{
    public string questID;
    public string text;
    public string activeID;
    public string deactiveID;
}
