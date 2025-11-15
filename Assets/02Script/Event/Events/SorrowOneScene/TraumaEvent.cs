using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;


/// <summary>
/// SorrowOne 귀신 등장 이벤트
/// </summary>
public class TraumaEvent : TriggerZone
{
    public GameObject enemy;       // 괴물 애니메이터
    public PlayableDirector traumaTimeline;
    
    public override void OnTrigger(GameObject actor)
    {
        if (!isContacted && initialDialog != null)
        //if (!isContacted)
        {
            // 귀신 등장음
            EventBus.Instance.Publish<GameEvents.PlaySFX>(new GameEvents.PlaySFX(SFXType.enemyLaugh));

            // 귀신 활성화
            enemy.SetActive(true);

            // 이벤트 모드 변경
            PlayerController.Instance.CurMode = GameMode.EventMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

            // 타임라인 + 대사이벤트
            StartCoroutine(TraumaTimeline());
        }
    }


    // 대화 끝난 후
    public override void OnEndDialog(UIEvents.EndDialog evt)
    {
        // 귀신 사라짐
        //StartCoroutine(FadeOutEnemy());

        // 마지막에!
        base.OnEndDialog(evt);
    }


    // 트라우마 귀신 등장 타임라인
    private IEnumerator TraumaTimeline() {
        yield return new WaitForSeconds(3f);        // 효과음 후 1초동안 대기

        // 카메라 필터 ON
        EventBus.Instance.Publish<GameEvents.FilterOn>(new GameEvents.FilterOn(FilterType.HorrorReverse, true));

        // 타임라인 시작
        traumaTimeline.Play();

        yield return new WaitUntil(() => traumaTimeline.state != PlayState.Playing);
        yield return null;

        // 카메라 필터 OFF
        EventBus.Instance.Publish<GameEvents.FilterOff>(new GameEvents.FilterOff(FilterType.HorrorReverse));

        // 괴물 비활성화
        enemy.SetActive(false);

        // 대화 이벤트 시작
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));
    }
}
