using UnityEngine;

public interface IInputHandler
{
    Vector2 GetMovement(); // 이동에 대한 입력값

    bool DoInteract(); // 상호작용

    bool TogglePopup(); // 팝업 열기/닫기

    bool Escape(); // ESC 메뉴 열기/닫기

    bool DoSelect(); // 선택지 선택

    bool Run(); // 달리기

    bool ToggleLight(); // 손전등 토글
}
