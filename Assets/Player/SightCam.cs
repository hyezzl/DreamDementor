using UnityEngine;
using UnityEngine.UI;

public class SightCam : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 1.5f;
    [SerializeField] Transform player;
    [SerializeField] private Button btn;

    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;

    private float cameraVertical = 0f;

    private bool iscontrol;

    private void Awake()
    {
        //transform.localPosition = new Vector3(0f, 1f, 0f);
    }

    private void OnEnable()
    {
        btn.onClick.AddListener(Toggle);
    }
    private void OnDisable()
    {
        btn.onClick.RemoveListener(Toggle);
    }

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        iscontrol = false;
    }

    private void Toggle() {
        iscontrol = !iscontrol;
        Debug.Log("Toggle 상태 변경: " + iscontrol);
        if (iscontrol)
        {
            mouseSensitivity = 1.5f;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else {
            mouseSensitivity = 0f;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void Update()
    {
        if (!iscontrol) return;

        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Y값은 카메라에 적용
        cameraVertical -= inputY;
        cameraVertical = Mathf.Clamp(cameraVertical, -minAngle, maxAngle);
        transform.localEulerAngles = Vector3.right * cameraVertical;

        // X값은 플레이어 자체 회전
        player.Rotate(Vector3.up * inputX);
    }
}
