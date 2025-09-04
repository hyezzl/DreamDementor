using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;
using static GameEvents;

public class PlayerMoveOne : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 1.5f;
    [SerializeField] private float minAngle = -60f;
    [SerializeField] private float maxAngle = 60f;

    [SerializeField] Transform player;


    private CharacterController cc;
    private IInputHandler inputHandler;
    private float cameraVertical = 0f;

    private bool isOne = false; // 1인칭인지?

    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;

    private void Awake()
    {
        if (!TryGetComponent<CharacterController>(out cc)) Debug.Log("PlayerMoveOne - Failed to Load CharacterController");
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.AspectChange>(OnAspectChange);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.AspectChange>(OnAspectChange);
    }

    private void OnAspectChange(GameEvents.AspectChange evt)
    {
        if (evt.mode == AspectMode.OnepersonMode)
            isOne = true;
        else
            isOne = false;
    }

    private void Update()
    {
        if (isOne)
        {
            HandleMouse();
            HandleMovement();
        }
    }


    private void HandleMouse()
    {
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Y값은 카메라에 적용
        cameraVertical -= inputY;
        cameraVertical = Mathf.Clamp(cameraVertical, -minAngle, maxAngle);
        transform.localEulerAngles = Vector3.right * cameraVertical;

        // X값은 플레이어 자체 회전
        player.Rotate(Vector3.up * inputX);
    }

    private void HandleMovement() {
        // todo :: PlayerState분기

        Vector2 moveDir = inputHandler.GetMovement().normalized;

        bool isRunning = inputHandler.Run();
        float speed = isRunning ? runSpeed : moveSpeed;

        // todo :: 중력적용

        cc.Move(moveDir * speed * Time.deltaTime);
    }
}