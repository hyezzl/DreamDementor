using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashLightTutorial : TriggerZone
{
    // 튜토리얼
    [SerializeField] private GameObject flashTuto;
    [SerializeField] private Button yesBTN;


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

    private IEnumerator PlayTuto()
    {
        // 튜토리얼 화면 나오게
        flashTuto.SetActive(true);

        // 확인 버튼 눌릴 때 까지 대기
        yield return WaitForSpace();

        flashTuto.SetActive(false);

        PlayerController.Instance.CurMode = GameMode.InspectMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
    }

    public override void OnEndDialog(UIEvents.EndDialog evt)
    {
        // 게임모드 변경
        if (evt.eventID == eventID)
        {
            // 튜토리얼 재생
            StartCoroutine(PlayTuto());

            Debug.Log($"{evt.eventID} 이벤트 트리거 삭제@");
            DestroyTrigger();
        }
    }
}


