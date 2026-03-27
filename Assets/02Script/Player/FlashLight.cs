using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 특정 맵에서만 적용됨
public class FlashLight : MonoBehaviour
{
    [Header("Light Setting")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private GameObject lightObj;

    [Header("Gauge Setting")]
    [SerializeField] private Image gaugeImg;
    [SerializeField] private float maxTime = 20f;
    [SerializeField] private float curGauge;        // 나중에 serial XX
    [SerializeField] private float recoverRatio = 1.5f; // 소모 대비 충전 비
    [SerializeField] private float uistayTime = 3f;

    private PlayerMove pm;
    private IInputHandler inputHandler;
    private Coroutine uiFadeCoroutine;      // 코루틴 중복 실행 방지


    private bool isOn = false;

    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;


    private void Start()
    {
        gaugeImg.gameObject.SetActive(false);
        curGauge = maxTime; // 완충 상태로 시작

        pm = GetComponent<PlayerMove>();
        if (pm == null) Debug.Log("FlashLight - Failed to Load PlayerMove Component");
    }

    private void Update()
    {
        // 입력 핸들러에 의한 손전등 토글
        if (inputHandler != null && inputHandler.ToggleLight()) {

            // 배터리가 있을 때만 켜기 가능
            if (!isOn && curGauge > 0)
            {
                HandleLight(true);
            }
            else if (isOn) {
                HandleLight(false);
            }
        }

        // 상태별 로직
        if (isOn)
        {
            ConsumeGauge();
            UpdateLightDir();
        }
        else {
            // 꺼져있을 때, 배터리가 풀이 아니라면 회복
            if (curGauge < maxTime) {
                RecoverGauge();
            }
        }
    }


    // 토글
    private void HandleLight(bool state) { 
        isOn = state;

        if (lightObj != null)
        {
            lightObj.SetActive(isOn);
        }

        // 손전등 토글 사운드

        if (isOn) ShowGaugeUI();
        else if (curGauge >= maxTime) { 
            // 껐는데 풀충전이라면 카운트 시작
            StartHideTimer();
        }
    }



    // 손전등 방향 갱신
    private void UpdateLightDir() {

        if (pm == null || lightObj == null) return;
        
        // 매 프레임 방향 갱신
        Vector3 dir = pm.PreDir;

        float rotationY = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        Quaternion rotate = Quaternion.Euler(46.42f, rotationY, 0);

        // 부드러운 회전
        lightObj.transform.rotation = Quaternion.Slerp(lightObj.transform.rotation, rotate, Time.deltaTime * rotationSpeed);
    }



    // 손전등 배터리 소모
    private void ConsumeGauge() { 
        curGauge -= Time.deltaTime;

        // 배터리 다쓰면 꺼짐
        if (curGauge <= 0) {
            curGauge = 0;
            HandleLight(false);
        }

        UpdateGaugeUI();
    }

    // 손전등 배터리 충전
    private void RecoverGauge() {
        curGauge += Time.deltaTime * recoverRatio;

        if (curGauge >= maxTime) {
            curGauge = maxTime;

            // 완충 순간부터 타이머 작동 시작
            StartCoroutine(HideUICoroutine());
        }

        UpdateGaugeUI();
    }


    // 손전등 게이지 갱신
    private void UpdateGaugeUI() {
        if (gaugeImg != null) {
            gaugeImg.fillAmount = curGauge / maxTime;
        }
    }


    //////// UI

    private void ShowGaugeUI() {
        if (uiFadeCoroutine != null) { 
            StopCoroutine(uiFadeCoroutine);
            uiFadeCoroutine = null;
        }

        if(!gaugeImg.gameObject.activeSelf) gaugeImg.gameObject.SetActive(true);
    }

    private void HideGaugeUI() {
        if (uiFadeCoroutine != null) StopCoroutine(uiFadeCoroutine);

        gaugeImg.gameObject.SetActive(false);
    }

    private void StartHideTimer() { 
        if(uiFadeCoroutine !=null) StopCoroutine(uiFadeCoroutine);
        uiFadeCoroutine = StartCoroutine(HideUICoroutine());
    }

    private IEnumerator HideUICoroutine() {
        // 3초동안 대기
        yield return new WaitForSeconds(uistayTime);

        // 다시 상태 체크
        if (!isOn && curGauge >= maxTime) {
            gaugeImg.gameObject.SetActive(false);
        }
        uiFadeCoroutine = null;
    }

}
