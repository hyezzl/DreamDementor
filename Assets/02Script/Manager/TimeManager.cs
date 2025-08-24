using UnityEngine;

/// <summary>
/// Ω√∞£ »Â∏ß ∞¸∏Æ
/// </summary>
public class TimeManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.GameModeChange>(OnModeChange);
    }

    private void OnDisable()
    {
        EventBus.Instance.Subscribe<GameEvents.GameModeChange>(OnModeChange);
    }

    private void OnModeChange(GameEvents.GameModeChange evt) {
        if (evt.mode == GameMode.DialogMode || evt.mode == GameMode.PauseMode)
        {
            StopTime();
        }
        else
            FlowTime();
    }

    private void StopTime() {
        Time.timeScale = 0;
        Debug.Log("Ω√∞£ ∏ÿ√„!!!");
    }

    private void FlowTime() {
        Time.timeScale = 1f;
    }
}
