using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HappyOneFirst : SceneStart
{
    [SerializeField] private GameObject happyOneTuto;
    [SerializeField] private CanvasGroup miniQuest;

    protected override void Awake()
    {
        SwitchSceneManager.Instance.CurScene = SceneType.HappyOneScene;
        base.Awake();
    }

    public override void Init(IDatabase db)
    { 
        base.Init(db);

        // 시작 시 1인칭 모드
        PlayerController.Instance.CurAspect = AspectMode.OnepersonMode;
        EventBus.Instance.Publish<GameEvents.AspectChange>(new GameEvents.AspectChange(AspectMode.OnepersonMode));

        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        SaveManager.Instance.SaveGame();
    }

    protected override void OnFirstVisit() { 
        base.OnFirstVisit();
    }

    protected override void OnSceneStart()
    {

        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(startEventID));
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(startEventID, initialDialog, GameMode.EventMode));
    }

    protected override void AfterDialog(UIEvents.EndDialog evt)
    {
        if (evt.eventID == startEventID)
        {
            // 첫 대화 이벤트 끝난 후
            StartCoroutine(PlayTuto());
        }
    }

    private IEnumerator PlayTuto()
    {
        // 튜토리얼 화면 나오게
        happyOneTuto.SetActive(true);

        // 확인버튼 눌릴 때 까지 대기
        yield return WaitForSpace();

        happyOneTuto.SetActive(false);

        // 퀘스트 생성
        miniQuest.alpha = 1f;

        // 게임모드 변경
        PlayerController.Instance.CurMode = GameMode.InspectMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
    }

    // 버튼
    private IEnumerator WaitForSpace()
    {
        // 3.5초 동안 스페이스 입력 무시 (딜레이)
        float delayTime = 3.3f;
        float timer = 0f;

        while (timer < delayTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 딜레이 후부터 스페이스 입력 대기
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
    }


}
