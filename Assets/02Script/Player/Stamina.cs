using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{

    [Header("Stamina Settings")]
    [SerializeField] private float decreaseRate = 33f;       // 1초당 쓸 스태미나값
    [SerializeField] private float recoverRate = 33f;         // 1초당 회복되는 스태미나값
    [SerializeField] private float recoverDelay = 1f;       // 멈춘 후 대기시간
    [SerializeField] private float minStaminaVal = 33f;     // 달릴 수 있는 스태미너의 최소값
    // 비활성화된 느낌을 줘야할것같은데.

    [Header("UI Refs")]
    [SerializeField] private Slider staminaBar;
    [SerializeField] private TextMeshProUGUI staminaText;

    private float maxStamina = 100f;
    private float curStamina;                   // 현재 스태미나
    private float delayTimer = 0f;

    private bool isSprint = false;              // 달리는 중인지
    private bool isRecovering = false;          // 회복중인지
    private bool staminaLow = false;            // 스태미나 부족 시 달릴 수 없음
    private bool canSprint = true;              // 달릴 수 있는 상태인지
    private bool forceRunning = false;

    private PlayerController pc;
    private PlayerMove pm;          // PlayerMove의 플래그변수 동기화 위함
    private IInputHandler inputHandler;

    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("Stamina - Failed to Load PlayerController");

        if (!TryGetComponent<PlayerMove>(out pm)) Debug.Log("Stamina - Failed to Load PlayerMove");

        curStamina = maxStamina;
    }

    private void Update()
    {
        if (inputHandler == null) return;
        if (pm == null) return;

        // 달릴 수 있는 상태인지?
        //canSprint = !staminaLow && inputHandler.GetMovement().sqrMagnitude > 0.1f;
        canSprint = pm.canRunning;
        this.forceRunning = pm.forceRunning;

        // 달리지못하는 상태를 동기화시켜 게이지에도 반영
        if (!canSprint || forceRunning) {
            if (isSprint) {
                isSprint = false;
                isRecovering = false;
                EventBus.Instance.Publish<GameEvents.OnSprintStop>(new GameEvents.OnSprintStop());
            }
            HandleRecoveryDelay();
            UpdateUI();
            return;
        }

        // 달리는 중
        if (inputHandler.Run() && inputHandler.GetMovement().sqrMagnitude > 0.1f)
        {
            if (!isSprint)
            {
                isSprint = true;
                isRecovering = false;
                // 달리기 시작 이벤트
                EventBus.Instance.Publish<GameEvents.OnSprintStart>(new GameEvents.OnSprintStart());
            }
            UseStamina();
            delayTimer = recoverDelay;  // 멈추는 순간 대기 타이머 초기화
        }
        else
        {
            if (isSprint)
            {
                isSprint = false;
                // 달리기 멈춤 이벤트
                EventBus.Instance.Publish<GameEvents.OnSprintStop>(new GameEvents.OnSprintStop());
            }
            HandleRecoveryDelay();
        }

        UpdateUI();
    }

    private void UseStamina() 
    {
        // 스태미너 사용
        curStamina -= decreaseRate * Time.deltaTime;
        EventBus.Instance.Publish<GameEvents.OnStaminaChanged>(new GameEvents.OnStaminaChanged(curStamina, maxStamina));
        
        if (curStamina <= 0) {
            curStamina = 0f;
            staminaLow = true;      // 스태미너가 부족해 달릴 수 없음
            isSprint = false;
            isRecovering = false;
            delayTimer = recoverDelay;

            // 뛰지 못하게 됨
            EventBus.Instance.Publish<GameEvents.OnStaminaDepleted>(new GameEvents.OnStaminaDepleted());
        }
    }

    

    private void HandleRecoveryDelay() {
        if (delayTimer > 0) {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0) {
                // 리커버리 가능
                StartRecovery();
            }
        }

        if (isRecovering) {
            RecoverStamina();
        }
    }

    // 스태미나 충전 시작
    private void StartRecovery() {
        isRecovering = true;
        EventBus.Instance.Publish<GameEvents.OnStaminaRecoverStart>(new GameEvents.OnStaminaRecoverStart());
    }


    private void RecoverStamina() {
        curStamina += recoverRate * Time.deltaTime;
        EventBus.Instance.Publish<GameEvents.OnStaminaChanged>(new GameEvents.OnStaminaChanged(curStamina, maxStamina));

        // 최소 스태미너 이상 회복되면 다시 달리기 허용
        if (staminaLow && curStamina >= minStaminaVal) {
            staminaLow = false;
        }

        // 모두 회복 시
        if (curStamina >= maxStamina) 
        {
            curStamina = maxStamina;
            isRecovering = false;
            // 스태미나가 모두 충전됐음을 알림
            EventBus.Instance.Publish<GameEvents.OnStaminaRecovered>(new GameEvents.OnStaminaRecovered());
        }
    }


    private void UpdateUI() {
        if (staminaBar != null) { 
            staminaBar.value = curStamina / maxStamina;
        }
        if (staminaText != null) {
            staminaText.text = curStamina + " / 100";
        }
    }

    //private void OnChangeSprintState(GameEvents.ChangeSprintState evt) {
    //    canSprint = evt.canRunning;
    //    staminaLow = !evt.canRunning;

    //    if (!canSprint && isSprint) {
    //        // 강제로 달리기 중지
    //        isSprint = false;
    //        isRecovering = false;
    //        delayTimer = recoverDelay;
    //        // 0이되는거맞나..?
    //        EventBus.Instance.Publish<GameEvents.OnStaminaDepleted>(new GameEvents.OnStaminaDepleted());
    //    }
    //}
}
