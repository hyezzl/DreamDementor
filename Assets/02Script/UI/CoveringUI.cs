using System.Collections;
using UnityEngine;

public class CoveringUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;

    private SceneType curScene;
    private GameMode curMode;

    private void OnEnable()
    {
        // 초기값
        group.alpha = 0f;

        EventBus.Instance.Subscribe<GameEvents.GameModeChange>(OnModeChange);
        EventBus.Instance.Subscribe<GameEvents.SceneStart>(OnNewScene);
    }
    private void OnDisable() {
        EventBus.Instance.Unsubscribe<GameEvents.GameModeChange>(OnModeChange);
        EventBus.Instance.Unsubscribe<GameEvents.SceneStart>(OnNewScene);
    }

    private void OnModeChange(GameEvents.GameModeChange evt) {
        curMode = evt.mode;
        UpdateCovering();
    }

    private void OnNewScene(GameEvents.SceneStart evt) {
        curScene = evt.curScene;
        UpdateCovering();
    }

    // 우선순위 지정을 위한 두 조건의 합집합
    private void UpdateCovering() {
        // RestScene에서는 무조건 숨김
        if (curScene == SceneType.RestScene || curScene == SceneType.TutorialScene ||
            curScene == SceneType.TitleScene)
        {
            Debug.Log($"현재씬은 {curScene} 이라서 UI숨김!!!!");
            ShowUI(false);
            return;
        }

        // 그 외 씬에서는 특정 모드일때만 보임
        switch (curMode)
        {
            case GameMode.InspectMode:
            case GameMode.EventInInspectMode:
                ShowUI(true);
                break;
            default:
                ShowUI(false);
                break;
        }
    }


    private void ShowUI(bool isOn) {
        group.alpha = isOn ? 1f : 0f;
        group.interactable = isOn;
        group.blocksRaycasts = isOn;
    }
}
