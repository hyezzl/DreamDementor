using System.Collections;
using UnityEngine;

public class CoveringUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.GameModeChange>(OnModeChange);
    }
    private void OnDisable() { 
        EventBus.Instance.Unsubscribe<GameEvents.GameModeChange>(OnModeChange);
    }

    private void OnModeChange(GameEvents.GameModeChange evt) {
        switch (evt.mode) {
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
