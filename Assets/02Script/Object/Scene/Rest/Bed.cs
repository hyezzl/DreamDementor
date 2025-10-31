using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : NPC
{
    protected override void OnEndNpcDialog(UIEvents.EndNpcDialog evt)
    {
        base.OnEndNpcDialog(evt);
        //if (evt.npcID == npcID && evt.npcEventID == curNpcEventID)
        if (evt.npcID == npcID)
        {
            // 강제 이벤트 발행
            StartCoroutine(AfterDialog());
        }
    }

    private IEnumerator AfterDialog() {
        // 이벤트 모드로 변경
        pc.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));
        
        yield return null;

        EventBus.Instance.Publish<GameEvents.SwitchScene>(new GameEvents.SwitchScene(SceneType.RestScene, SceneType.SorrowScene));
    }
}
