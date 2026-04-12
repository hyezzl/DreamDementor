using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class H_PlayTimeline03 : TriggerZone
{
    [Header("Timeline03")]
    [SerializeField] private PlayableDirector timeline03;
    [SerializeField] private GameObject enemy;

    public override void OnTrigger(GameObject actor)
    {
        if (!isContacted)
        {
            PlayerController.Instance.CurMode = GameMode.EventMode;
            EventBus.Instance.Publish(new GameEvents.GameModeChange(GameMode.EventMode));

            enemy?.SetActive(true);

            StartCoroutine(PlayTimeline03());
        }
    }

    private IEnumerator PlayTimeline03()
    { 
        if(timeline03 == null) yield break;

        timeline03.Play();

        while (timeline03.state == PlayState.Playing)
        { yield return null; }

        // 타임라인 끝나면 술래잡기 시작
        PlayerController.Instance.CurMode = GameMode.InspectMode;
        EventBus.Instance.Publish(new GameEvents.GameModeChange(GameMode.InspectMode));

        // 괴물 바로 Chase
        EventBus.Instance.Publish(new GameEvents.EnemyStateChange(EnemyState.Chase));

        DestroyTrigger();
    }
}
