using UnityEngine;

public class PauseMode : MonoBehaviour
{
    [Header("UI Ref")]
    [SerializeField] private RectTransform pause;

    private PlayerController pc;
    private IInputHandler inputHandler;
    private bool isPaused = false;
    private GameMode preMode; // 이전 모드 캐싱값

    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("PauseMode - Failed to Load PlayerController");
    }

    private void Update()
    {
        TogglePause();
    }

    private void TogglePause() {
        if (inputHandler.Escape()) {

            // todo : 캐릭터 모드 조건 추가
            //게임 모드 변경
            if (isPaused)
            {
                Convert();
                EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(preMode));
            }
            else {
                preMode = pc.CurMode; // 캐싱
                Convert();
                EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.PauseMode));
            }
        }
    }

    // 외부 호출용
    public void ExitPause() {
        Convert();
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(preMode));
    }

    private void Convert() {
        isPaused = !isPaused;
        pause.gameObject.SetActive(isPaused);
        //Time.timeScale = isPaused ? 0 : 1; (시간 멈추기)
    }
}
