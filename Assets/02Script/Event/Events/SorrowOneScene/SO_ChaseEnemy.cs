using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class SO_ChaseEnemy : MonoBehaviour
{
    public string eventID;

    [Header("AppearEnemy Timeline")]
    public PlayableDirector appearEnemyTimeline;

    [Header("Enemy")]
    public GameObject enemy;

    // 타임라인 이후 실행할 이벤트
    private IDatabase database;
    private Dictionary<string, Dictionary<int, DialogData>> allDialogs;
    private Dictionary<int, DialogData> initialDialog;

    public void Init(IDatabase db)
    {
        database = db;
        allDialogs = database.GetDialogEvent(eventID);
        initialDialog = database.GetDialog(eventID, eventID);
        if (initialDialog == null) Debug.Log("SO_ChaseEnemy - Failed to Load Dialog");
    }


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PuzzleEvents.SO_GetAllNumber>(ReadyToStart);
        EventBus.Instance.Subscribe<PuzzleEvents.SO_EndChase>(EndChase);

        enemy.SetActive(false);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PuzzleEvents.SO_GetAllNumber>(ReadyToStart);
        EventBus.Instance.Unsubscribe<PuzzleEvents.SO_EndChase>(EndChase);
    }

    private void ReadyToStart(PuzzleEvents.SO_GetAllNumber evt)
    {
        StartCoroutine(TimelineSequence());
    }

    private IEnumerator TimelineSequence()
    {
        // 모드 변경
        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        // 브금 변경

        yield return null;

        // 타임라인 재생
        if (appearEnemyTimeline != null) { 
            appearEnemyTimeline.Play();

            yield return new WaitForSeconds((float)appearEnemyTimeline.duration);
        }

        yield return null;

        // 타임라인 재생 후 이벤트 저장
        EventBus.Instance.Publish<UIEvents.EndDialog>(new UIEvents.EndDialog("E053"));

        // 술래잡기 시작
        enemy.SetActive(true);
        EventBus.Instance.Publish<GameEvents.EnemyStateChange>(new GameEvents.EnemyStateChange(EnemyState.Chase));
        
        // 타임어택 시작
        EventBus.Instance.Publish<PuzzleEvents.SO_TimeAttack>(new PuzzleEvents.SO_TimeAttack());

        // 모드 변경
        PlayerController.Instance.CurMode = GameMode.InspectMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));


    }

    private void EndChase(PuzzleEvents.SO_EndChase evt) {
        // 타임어택이 끝났으므로 적 삭제
        if (enemy != null) { 
            enemy.SetActive(false);
        }

        // 이벤트
        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        if (initialDialog != null)
        {
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));
        }
    }
}
