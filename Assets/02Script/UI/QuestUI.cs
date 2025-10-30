using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using static GameEvents;

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
        EventBus.Instance.Subscribe<GameEvents.EndEvent>(OnEndEvent);
        EventBus.Instance.Subscribe<UIEvents.EndDialog>(OnEndDialog);
        EventBus.Instance.Subscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);

    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GameModeChange>(OnModeChange);
        EventBus.Instance.Subscribe<GameEvents.EndEvent>(OnEndEvent);

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
