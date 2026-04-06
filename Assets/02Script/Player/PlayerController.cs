
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
    EventMode,
    ChasedMode,
    EventInInspectMode,      // 걷기만 가능
    GameOverMode,
    UIPuzzleMode,       // UI 통한 마우스 + 드래그 퍼즐
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
/// 
public class PlayerController : Singleton<PlayerController>
{
    public int CurHP { get; private set; }

    // 스테이지별 정신력 관리
    public Dictionary<Stage, int> wholeHP = new();
    public PlayerState CurState { get; set; } = PlayerState.Idle;
    public GameMode CurMode { get; set; } = GameMode.InspectMode;
    public AspectMode CurAspect { get; set; } = AspectMode.ThirdpersonMode;



    // 초기화
    protected override void DoAwake()
    {
        base.DoAwake();

        // HP 기본값 세팅
        if (CurHP <= 0)
        {
            CurHP = 100;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) { 
            StateReport();
        }
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.OnDamaged>(OnDamaged);
        EventBus.Instance.Subscribe<GameEvents.OnHeal>(OnHeal);

        EventBus.Instance.Subscribe<GameEvents.NewStageStart>(OnStartNewStage);
        EventBus.Instance.Subscribe<GameEvents.StageEnd>(OnEndStage);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.OnDamaged>(OnDamaged);
        EventBus.Instance.Unsubscribe<GameEvents.OnHeal>(OnHeal);

        EventBus.Instance.Unsubscribe<GameEvents.NewStageStart>(OnStartNewStage);
        EventBus.Instance.Unsubscribe<GameEvents.StageEnd>(OnEndStage);
            }

    private void OnDamaged(GameEvents.OnDamaged evt) {
        //ChangeHp(Scene2Stage(CurScene), evt.damage);
        ChangeHp(evt.damage); // 바로 사용

    }

    private void OnHeal(GameEvents.OnHeal evt) {
        //ChangeHp(Scene2Stage(CurScene), evt.heal);

        SceneType curScene = SwitchSceneManager.Instance.CurScene;
        ChangeHp(evt.heal);
    }

    // 스테이지 시작 시
    private void OnStartNewStage(GameEvents.NewStageStart evt) { 
        
    }


    // 스테이지 종료
    private void OnEndStage(GameEvents.StageEnd evt) {
        // 종료스테이지 체력값 저장
        Debug.Log($"{evt.stage} 스테이지 종료! 체력값 ({CurHP}) 저장!");
        wholeHP[evt.stage] = CurHP;

        // 저장 이후 100으로 (Next Stage)
        CurHP = 100;
    }

    // 음수면 피해, 양수면 회복
    public void ChangeHp(int val)
    {
        int preHp = CurHP;
        int newHp = Mathf.Clamp(preHp + val, 0, 100);
        CurHP = newHp;

        Debug.Log($"~~~~preHp : {preHp} -> new : {newHp} , val : {val}");

        // 정신력이 30이하로 도달 (기존 30초과 -> 30이하)
        if (newHp <= 30 && preHp > 30) 
        {
            SceneType curScene = SwitchSceneManager.Instance.CurScene;
            EventBus.Instance.Publish<GameEvents.OnLackedHP>(new GameEvents.OnLackedHP(curScene));
            Debug.Log("피가 30이하가 되어 그로기상태");
        }

        // 정신력이 안정권으로 도달 (기존 30이하 -> 30초과)
        if (newHp > 30 && preHp <= 30) {
            SceneType curScene = SwitchSceneManager.Instance.CurScene;
            EventBus.Instance.Publish<GameEvents.OnSteadyHp>(new GameEvents.OnSteadyHp(curScene));
        }

        // 정신력 0 도달 (씬알려주기) -> 게임오버
        if (newHp <= 0 && preHp > 0)
        {
            SceneType curScene = SwitchSceneManager.Instance.CurScene;
            EventBus.Instance.Publish<GameEvents.OnHpDepeleted>(new GameEvents.OnHpDepeleted(curScene));

            // 게임오버
            EventBus.Instance.Publish<GameEvents.GameOver>(new GameEvents.GameOver(curScene, DeathType.BadChoice));
        }

        // HP 변경
        if (val != 0)
        {
            SceneType curScene = SwitchSceneManager.Instance.CurScene;
            EventBus.Instance.Publish<GameEvents.OnHpChange>(new GameEvents.OnHpChange(Scene2Stage(curScene), preHp, newHp));
        }
    }

    // 외부에서 CurHP Set
    public void SetCurHP(int hp)
    {
        int diff = hp - CurHP;
        if (diff != 0) {
            ChangeHp(diff);
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

            case SceneType.HorrorScene:
                return Stage.Horror;

            default:
                return Stage.Happy;
        }
    }


    // 플레이어의 현상태 브리핑 함수
    private void StateReport() {
        Debug.Log($"curHP : {CurHP}");
        Debug.Log($"curMode : {CurMode}");
        Debug.Log($"curAspect : {CurAspect}");
        Debug.Log($"curScene : {SwitchSceneManager.Instance.CurScene}");
        Debug.Log($"curState : {CurState}");
    }
}
