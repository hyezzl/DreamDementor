
/// <summary>
/// 플레이어 상태
/// </summary>
public enum PlayerState
{ 
    Idle,
    Walk,
    Run,
    //Crouching? Jump?
}


/// <summary>
/// 게임 모드
/// </summary>
public enum GameMode
{ 
    InspectMode,
    PauseMode,
    DialogMode,
    NarrativeMode,
    ChasedMode,
    EventMode,
}

/// <summary>
/// 현재 게임 인칭모드
/// </summary>
public enum AspectMode
{ 
    ThirdpersonMode,  // 3인칭 ( 2.5D )
    OnepersonMode,    // 1인칭
}


public class PlayerController : Singleton<PlayerController>
{
    public PlayerState CurState { get; set; } = PlayerState.Idle;
    public GameMode CurMode { get; set; } = GameMode.InspectMode;
    public AspectMode CurAspect { get; set; } = AspectMode.ThirdpersonMode;
}
