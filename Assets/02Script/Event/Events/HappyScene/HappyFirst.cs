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

    private int[] requireIDs = { 10001002, 10001003, 10001004 };

    protected override void OnFirstVisit() {
        base.OnFirstVisit();
        PlayTeleport();
    }

    protected override void OnRevisit()
    {
        base.OnRevisit();
        pp.gameObject.SetActive(false);

        // 재방문 + 열쇠 3개 모두 가지고있을경우   
        if (IsSatisfying()) {
            // 19번 이벤트 발행
            //EventBus.Instance.Publish<GameEvents.PlayEvent>();
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog("E019", initialDialog));
        }
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


    // 열쇠 3개가 모두 있는지 
    private bool IsSatisfying()
    {
        // 인벤토리 가져옴
        var inventory = InventoryManager.Instance.GetInventory();

        foreach (var id in requireIDs)
        {
            bool hasKey = false;
            foreach (var item in inventory)
            {
                if (item.itemID == id)
                {
                    hasKey = true;
                    break;
                }
            }
            if (!hasKey)
                return false;   // 하나라도 없으면 false
        }
        return true;
    }
}
