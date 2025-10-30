using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour
{

    [Header("UI Ref")]
    [SerializeField] private CanvasGroup canvas;

    private void Awake()
    {
        canvas.alpha = 0f;
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.GameModeChange>(OnModeChange);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GameModeChange>(OnModeChange);
    }






    // 모드 변경 시 숨김
    private void OnModeChange(GameEvents.GameModeChange evt) {
        if (evt.mode == GameMode.InspectMode || evt.mode == GameMode.PauseMode)
        {
            canvas.alpha = 1f;
        }
        else 
        {
            // Inspect모드 제외 + 대화 + 이벤트 + 게임오버
            canvas.alpha = 0f;
        }
    }
}
