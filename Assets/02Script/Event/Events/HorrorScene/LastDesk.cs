using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LastDesk : CustomItem
{
    [Header("Timeline04")]
    [SerializeField] private PlayableDirector timeline04;

    public override void Interact() 
    {
        Debug.Log("왜않되");
        StartCoroutine(PlayEnding());

        // 이벤트모드
        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish(new GameEvents.GameModeChange(GameMode.EventMode));
    }

    private IEnumerator PlayEnding()
    { 
        if(timeline04 == null) yield break;

        // 효과가 있었음 좋겠다

        yield return null;
        // 워프?

        timeline04.Play();

        while(timeline04.state == PlayState.Playing) yield return null;

        // 타임라인 end

    }
}
