using UnityEngine;

public class JoystickInputHandler : IInputHandler
{
    private FloatingJoystick joystick; // Fixed? Floating?

    public bool isInteractClicked = false;
    public bool isEscapeClicked = false;
    public bool isPopupClicked = false;
    public bool isLightClicked = false;


    // DI (생성자)
    public JoystickInputHandler(FloatingJoystick joystick)
    {
        this.joystick = joystick;
    }

    public Vector2 GetMovement() => new Vector2(joystick.Horizontal, joystick.Vertical);

    // UI에서는 Interact == Select
    public bool DoInteract()
    {
        bool result = isInteractClicked; // 외부에서 들어온 true값 캐싱
        isInteractClicked = false; // 초기화
        return result;
    }

    public bool DoSelect()
    {
        bool result = isInteractClicked; // 외부에서 들어온 true값 캐싱
        isInteractClicked = false; // 초기화
        return result;
    }

    public bool Escape()
    {
        bool result = isEscapeClicked;
        isEscapeClicked = false;
        return result;
    }


    public bool Run()
    {
        // 조이스틱값이 특정값 이상일때!
        Vector2 input = new Vector2(joystick.Horizontal, joystick.Vertical);
        return input.magnitude >= 0.6f; // 수치조정하기!!!!!!!!!

    }

    public bool TogglePopup()
    {
        bool result = isPopupClicked;
        isPopupClicked = false;
        return result;
    }

    public bool ToggleLight()
    {
        bool result = isLightClicked;
        isLightClicked = false;
        return result;
    }
}
