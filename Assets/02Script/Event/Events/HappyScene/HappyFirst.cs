using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Timeline;

public class HappyFirst : SceneStart
{
    [SerializeField] PlayableDirector eyesOpen;
    [SerializeField] private Canvas eyeCanvas;
    [SerializeField] private PostProcessVolume pp;


    protected override void OnFirstVisit() {
        base.OnFirstVisit();
        PlayTeleport();
    }

    protected override void OnRevisit()
    {
        base.OnRevisit();
        pp.gameObject.SetActive(false);
    }


    private void PlayTeleport()
    {
        StartCoroutine(PlayEvents());
    }

    private IEnumerator PlayEvents()
    {
        pc.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        // 눈열리는 연출
        eyesOpen.Play();
        yield return new WaitUntil(() => eyesOpen.state != PlayState.Playing);

        // 이벤트 시작 (대화)
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(startEventID));

        // 대화모드
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(startEventID, initialDialog, GameMode.InspectMode));
    }
}
