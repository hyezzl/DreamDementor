using UnityEngine;

public class KeyboardInputHandler : MonoBehaviour, IInputHandler
{
    public Vector2 GetMovement()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
