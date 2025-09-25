using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{

    [Header("Stamina Settings")]
    [SerializeField] private float consumeVal = 33f;    // 1초당 쓸 스태미나값
    [SerializeField] private float recoverDelay = 3f;   // 멈춘 후 대기시간
    [SerializeField] private float recoverSpeed = 33f;   // 1초당 회복되는 스태미나값
    [SerializeField] private float minStaminaVal = 33f;  // 달릴 수 있는 스태미너의 최소값
    // 비활성화된 느낌을 줘야할것같은데.

    [Header("UI Refs")]
    [SerializeField] private Slider staminaBar;
    [SerializeField] private TextMeshProUGUI staminaText;

    private float maxStamina = 100f;
    private float curStamina;  // 현재 스태미나
    private float delayTimer = 0f;

    private bool isRunning = false;     // 달리는 중인지
    private bool canRecovering = false;  // 회복가능한 상태인지

    private PlayerController pc;
    private IInputHandler inputHandler;

    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;


    private void Awake()
    {
        //if (!TryGetComponent<PlayerController>(out pc)) Debug.Log("Stamina - Failed to Load PlayerController");
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("Stamina - Failed to Load PlayerController");
    }

    private void Update()
    {
        if (inputHandler.Run())
        {
            UseStamina();
            canRecovering = false;
            delayTimer = recoverDelay;  // 멈추는 순간 대기 타이머 초기화;
        }
        else
        {
            // 회복 대기 시간 감소
            if (delayTimer > 0f) { 
                delayTimer -= Time.deltaTime;
                if (delayTimer <= 0 && curStamina >= minStaminaVal) {
                    canRecovering = true;
                    EventBus.Instance.Publish<GameEvents.StaminaRecovered>(new GameEvents.StaminaRecovered());
                }
            }

            if (canRecovering)
                Recovering();
        }
        UpdateUI();
    }

    private void UseStamina() {
        // 조건
        isRunning = true;
        delayTimer = 0f;

        // 스태미너 사용
        curStamina -= consumeVal * Time.deltaTime;
        
        if (curStamina <= 0) {
            curStamina = 0f;

            // 뛰지 못하게 됨
            //EventBus.Instance.Publish<GameEvents.StaminaDepleted>(new GameEvents.StaminaDepleted());
            
            isRunning = false;
            canRecovering = false;
            delayTimer = recoverDelay;
        }
    }

    // 회복 대기
    private void Recovering() {
        isRunning = false;

        if (canRecovering && curStamina <= maxStamina) {
            curStamina += recoverSpeed * Time.deltaTime;

            if (curStamina > maxStamina) 
            {
                curStamina = maxStamina;
            }
        }
    }

    private void CanRecover() {
        //update
        if (delayTimer > 0f) {
            delayTimer -= Time.deltaTime;
        }

        if (delayTimer <= 0f && curStamina >= minStaminaVal) {
            canRecovering = true;
            // 달리기 가능
            //EventBus.Instance.Publish<GameEvents.StaminaRecoverd>(new GameEvents.StaminaRecoverd());
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
}
