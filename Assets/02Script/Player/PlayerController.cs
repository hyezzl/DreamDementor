
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static GameEvents;

/// <summary>
/// 플레이어 상태
/// </summary>
public enum PlayerState
{ 
    Idle,
    Walk,
    Run,
}


/// <summary>
/// 게임 모드
/// </summary>
public enum GameMode
{ 
    InspectMode,
    PauseMode,          // Popup
    DialogMode,
    NarrativeMode,
    EventMode,
    ChasedMode,
    EventInInspectMode,
    GameOverMode,
    None,
}

/// <summary>
/// 현재 게임 인칭모드
/// </summary>
public enum AspectMode
{ 
    ThirdpersonMode,  // 3인칭 ( 2.5D )
    OnepersonMode,    // 1인칭
}

/// <summary>
/// 점수 합산을 위한 큰 4개의 스테이지 씬
/// </summary>
public enum Stage
{ 
    Happy,
    Sorrow,
    Chaos,
    Horror,
}

/// <summary>
/// 플레이어의 현재 Hp(정신력) 관리 + 현재 상태 / 현재 게임모드 / 현재 인칭 관리
/// </summary>
public class PlayerController : Singleton<PlayerController>
{
    // 플레이어 정신력 관리
    public Dictionary<Stage, int> curHp = new();

    public PlayerState CurState { get; set; } = PlayerState.Idle;
    public GameMode CurMode { get; set; } = GameMode.InspectMode;
    public AspectMode CurAspect { get; set; } = AspectMode.ThirdpersonMode;
    public SceneType CurScene { get; set; } = SceneType.HappyScene;



    // 초기화
    protected override void DoAwake()
    {
        base.DoAwake();

        // HP 초기화
        curHp[Stage.Happy] = 100;
        curHp[Stage.Sorrow] = 100;
        curHp[Stage.Chaos] = 100;
        curHp[Stage.Horror] = 100;
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.OnDamaged>(OnDamaged);
        EventBus.Instance.Subscribe<GameEvents.OnHeal>(OnHeal);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.OnDamaged>(OnDamaged);
        EventBus.Instance.Unsubscribe<GameEvents.OnHeal>(OnHeal);
    }

    private void OnDamaged(GameEvents.OnDamaged evt) {
        //ChangeHp(Scene2Stage(evt.curScene), evt.damage);
        ChangeHp(Scene2Stage(CurScene), evt.damage);
    }

    private void OnHeal(GameEvents.OnHeal evt) {
        //ChangeHp(Scene2Stage(evt.curScene), evt.heal);
        ChangeHp(Scene2Stage(CurScene), evt.heal);
    }

    // 체력 관리
    public void ChangeHp(Stage stage, int val)  // 음수면 피해, 양수면 회복
    {
        if (!curHp.ContainsKey(stage)) {

            Debug.Log("존재하지않는 스테이지");
            return;
        }

        int preHp = curHp[stage];
        int newHp = Mathf.Clamp(preHp + val, 0, 100);
        curHp[stage] = newHp;

        EventBus.Instance.Publish<GameEvents.OnHpChange>(new GameEvents.OnHpChange());

        // 정신력 0 도달 (씬알려주기) -> 게임오버
        if (newHp <= 0 && preHp > 0)
        {
            EventBus.Instance.Publish<GameEvents.OnHpDepeleted>(new GameEvents.OnHpDepeleted(CurScene));
        }

        // HP 변경
        if (val != 0) {
            EventBus.Instance.Publish<GameEvents.OnHpChange>(new GameEvents.OnHpChange(Scene2Stage(CurScene), preHp, newHp));
        }
    }


    // Scene -> Stage
    public Stage Scene2Stage(SceneType scene) {
        switch (scene) {
            case SceneType.HappyScene:
            case SceneType.HappyOneScene:
                return Stage.Happy;

            case SceneType.SorrowScene: 
            case SceneType.SorrowOneScene:
                return Stage.Sorrow;

            case SceneType.ChaosScene:
            case SceneType.ChaosOneScene:
                return Stage.Chaos;

            case SceneType.HorrorScene:
            case SceneType.HorrorOneScene:
                return Stage.Horror;

            default:
                return Stage.Happy;
        }
    }

}





