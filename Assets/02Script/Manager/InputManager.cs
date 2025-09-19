using UnityEngine;

/// <summary>
/// 유저의 입력을 총체관리
/// </summary>

public class InputManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private FloatingJoystick joystick; // 조이스틱 참조

    private CombineInputHandler inputHandler;

    private void Awake()
    {
        var keyboardHandler = new KeyboardInputHandler();
        var joystickHandler = new JoystickInputHandler(joystick);
        inputHandler = new CombineInputHandler(keyboardHandler, joystickHandler); // 두 입력수단 결합
        FindAnyObjectByType<UIInputBridge>().joyHandler = joystickHandler; // UIInputBridge에 조이스틱 핸들러 전달

        //SendInputVal();
    }

    private void Start()
    {
        SendInputVal();
    }

    // 외부에 입력 넘겨주기
    private void SendInputVal() {
        FindAnyObjectByType<PlayerMove>()?.SetInputHandler(inputHandler);
        FindAnyObjectByType<Interact>()?.SetInputHandler(inputHandler);  
        //FindAnyObjectByType<PauseMode>()?.SetInputHandler(inputHandler);
        FindAnyObjectByType<PopupManager>()?.SetInputHandler(inputHandler);
        FindAnyObjectByType<DialogPopup>()?.SetInputHandler(inputHandler);
        FindAnyObjectByType<MonologuePopup>()?.SetInputHandler(inputHandler);
        FindAnyObjectByType<PlayerMoveOne>()?.SetInputHandler(inputHandler);
    }
}
