using UnityEngine;

public class KeyboardInputHandler : IInputHandler
{
    public Vector2 GetMovement() =>
        new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // Raw?

    public bool DoInteract() => Input.GetKeyDown(KeyCode.Space); //Input.GetKeyDown(KeyCode.E) ||

    public bool TogglePopup() => Input.GetKeyDown(KeyCode.Tab);

    public bool Escape() => Input.GetKeyDown(KeyCode.Escape);

    public bool DoSelect() => Input.GetKeyDown(KeyCode.Space);

    public bool Run() => Input.GetKey(KeyCode.LeftShift);

    public bool ToggleLight() => Input.GetKeyDown(KeyCode.F);
}
