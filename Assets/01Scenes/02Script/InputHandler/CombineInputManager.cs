using UnityEngine;

public class CombineInputHandler : IInputHandler
{
    private IInputHandler keyboardHandler;
    private IInputHandler joystickHandler;

    // DI
    public CombineInputHandler(IInputHandler keyboardHandler, IInputHandler joystickHandler)
    {
        this.keyboardHandler = keyboardHandler;
        this.joystickHandler = joystickHandler;
    }
    public Vector2 GetMovement()
    {
        // 입력이 동시발생 했을 때, 키보드 우선
        Vector2 keyboardMovement = keyboardHandler.GetMovement();
        Vector2 joystickMovement = joystickHandler.GetMovement();
        return keyboardMovement.sqrMagnitude > 0.01f ? keyboardMovement : joystickMovement;
    }

    public bool DoInteract() => keyboardHandler.DoInteract() || joystickHandler.DoInteract();

    public bool DoSelect() => keyboardHandler.DoSelect() || joystickHandler.DoSelect();

    public bool Escape() => keyboardHandler.Escape() || joystickHandler.Escape();


    public bool Run() => keyboardHandler.Run() || joystickHandler.Run();

    public bool TogglePopup() => keyboardHandler.TogglePopup() || joystickHandler.TogglePopup();

    public bool ToggleLight() => keyboardHandler.ToggleLight() || joystickHandler.ToggleLight();
}

