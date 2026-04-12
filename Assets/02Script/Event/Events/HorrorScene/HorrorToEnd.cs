using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HorrorToEnd : EventBase
{
    public string illID;

    [Header("Timeline05")]
    [SerializeField] private PlayableDirector timeline05;

    [Header("Animators")]
    public Animator playerAnim;
    public Animator enemyAnim;

    protected override void PlayEvent(GameEvents.PlayEvent evt)
    {
        if (evt.eventID == eventID) { 
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.EventMode));
            // 애니메이션 강제
            ForceAnimation();
        }

    }

    protected override void CloseDialog(UIEvents.EndDialog evt)
    {
        if (evt.eventID == eventID)
        {
            // 대화끝나면 바로 다음 타임라인 재생
            StartCoroutine(PlayTimeline());
        }
    }


    private IEnumerator PlayTimeline()
    {
        if (timeline05 == null) yield break;

        timeline05.Play();

        while(timeline05.state == PlayState.Playing) yield return null;

        // 타임라인이 끝나면 엔딩씬

        // 일러스트 n초간 전시
        EventBus.Instance.Publish<GameEvents.OpenEndingScene>(new GameEvents.OpenEndingScene());
    }


    // 애니메이션 강제
    private void ForceAnimation()
    {
        if (playerAnim != null || enemyAnim != null)
        {
            playerAnim.SetFloat("lookX", -1f);
            playerAnim.SetFloat("lookY", 0f);
            enemyAnim.SetFloat("dirX", 0f);
            enemyAnim.SetFloat("dirY", 1f);
        }
    }
}
