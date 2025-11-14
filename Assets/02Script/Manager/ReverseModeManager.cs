using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


/// <summary>
/// 피가 30이하일 때 3인칭 맵에서의 이벤트  (HappyScene, SorrowScene, ..)
/// </summary>
public class ReverseModeManager : MonoBehaviour, IGameEvent
{
    public string eventID;

    private IDatabase database;
    protected Dictionary<int, DialogData> initialDialog;


    private bool isLackedHP = false;
    private bool isSpawn = false;       // 괴물이 스폰되었는지

    // 각자 맵에 존재할 괴물 등장 타임라인
    [Header("각 맵의 괴물등장 타임라인")]
    [SerializeField] private PlayableDirector appearance;   // 등장 타임라인

    [Header("Enemy")]
    [SerializeField] private GameObject enemy;
    [SerializeField] private Billboard billboard;


    public void Init(IDatabase db)
    {
        database = db;
        Debug.Log($"*{this.GetType().Name} 의 Init 완료!");

        initialDialog = database.GetDialog(eventID, eventID);
        if (initialDialog == null) Debug.Log($"*{this.GetType().Name} - Failed to Load DialogData");
    }



    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.OnLackedHP>(OnWarningState);

        EventBus.Instance.Subscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.OnLackedHP>(OnWarningState);

        EventBus.Instance.Unsubscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);
    }


    // 정신력 부족으로 인한 반전 상태 개시
    private void OnWarningState(GameEvents.OnLackedHP evt) {
        isLackedHP = true;
    }

    private void OnEndNpcDialog(UIEvents.EndNpcDialog evt) {
        if (isLackedHP && !isSpawn) {
            // 피낮음 + 해당 npc와의 대화가 끝났을 시에만 이벤트 발행

            // 이벤트 모드
            PlayerController.Instance.CurMode = GameMode.EventMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

            enemy.SetActive(true);  // 괴물 소환
            StartCoroutine(SpawnDementor());

            // 초기화
            isLackedHP = false;
            isSpawn = true;
        }
    }

    private IEnumerator SpawnDementor() {
        Debug.Log("괴물 등장!!!!!!!");
        billboard.enabled = false;

        // 필터
        //      2초동안 글리치
        EventBus.Instance.Publish<GameEvents.FilterOn>
            (new GameEvents.FilterOn(FilterType.Glitch, false, 4f));
        //      호러인버스
        EventBus.Instance.Publish<GameEvents.FilterOn>(new GameEvents.FilterOn(FilterType.HorrorReverse, true));

        // 효과음
        EventBus.Instance.Publish<GameEvents.PlaySFX>(new GameEvents.PlaySFX(SFXType.enemyScream));

        // 타임라인 시작
        appearance.Play();

        yield return new WaitUntil(() => appearance.state != PlayState.Playing);

        // 이벤트 발행
        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent("E043"));
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode)); ;

        // 괴물 상태 변경
        EventBus.Instance.Publish<GameEvents.EnemyStateChange>(new GameEvents.EnemyStateChange(EnemyState.Chase));
        billboard.enabled = true;
        Debug.Log("술래잡기 시작!!!");
    }

}
