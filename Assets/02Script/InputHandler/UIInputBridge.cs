using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 버튼입력(onclick) 이벤트를 JoystickInputHandler로 전달하는 브릿지 클래스

public class UIInputBridge : MonoBehaviour
{
    public JoystickInputHandler joyHandler;

    [Header("TempBTN Refs")]
    [SerializeField] private Button interactBTN;
    [SerializeField] private Button popupBTN;
    [SerializeField] private Button lightBTN;
    [SerializeField] private Button menuBTN;

    private void Start()
    {
        if (joyHandler != null)
        {
            interactBTN.onClick.AddListener(() => { joyHandler.isInteractClicked = true; EventSystem.current.SetSelectedGameObject(null); });
            popupBTN.onClick.AddListener(() => { joyHandler.isPopupClicked = true; EventSystem.current.SetSelectedGameObject(null); });
            lightBTN.onClick.AddListener(() => { joyHandler.isLightClicked = true; EventSystem.current.SetSelectedGameObject(null); });
            menuBTN.onClick.AddListener(() => { joyHandler.isEscapeClicked = true; EventSystem.current.SetSelectedGameObject(null); });
        }
        else { Debug.Log("UIInputBridge - Failed to Load JoyStickInputHandler"); }
    }
}
