using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappySelections : MonoBehaviour
{
    private PlayerController pc;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log ("HappySelections - Failed to Load PlayerController");
    }


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.MakeChoice>(OnMakeChoice);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.MakeChoice>(OnMakeChoice);
    }


    private void OnMakeChoice(UIEvents.MakeChoice evt) {
        // 임시

        
        // 해피씬에서의 선택지

        switch (evt.choiceID) {
            case "C002":
                Debug.Log("선택지 C002 완료");

                pc.CurMode = GameMode.InspectMode;
                EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));

                break;

            case "C004":
                Debug.Log("선택지 C003 완료");

                pc.CurMode = GameMode.InspectMode;
                EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));

                break;
        }
    }
}
