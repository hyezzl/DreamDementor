using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class SOTimeAttack : MonoBehaviour
{
    [SerializeField] private int timeLimit = 30;    // 제한시간(초)

    [Header("UI Ref")]
    [SerializeField] private CanvasGroup timeGroup;
    [SerializeField] private TextMeshProUGUI secText;   // 초
    [SerializeField] private TextMeshProUGUI csText;    // 센티초

    [Header("Object Interaction")]
    [SerializeField] private GameObject wall;

    [Header("Timeline Ref")]
    [SerializeField] private PlayableDirector wallTimeline;

    private void Start()
    {
        wall.SetActive(false);  // 시작 시 벽 비활성화
        timeGroup.alpha = 0f;   // 시작 시 시간 비활성화 

        // 임시 시작
        //StartCoroutine(temp());
    }

    private bool isOn = false;       // 타임어택 시작
    private bool isChase = false;    // 귀신에게 쫓기는지
    private float nowTime;      // 남은시간 (현재 표시될 시간)


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PuzzleEvents.SO_TimeAttack>(TriggerTA);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PuzzleEvents.SO_TimeAttack>(TriggerTA);
    }

    private void TriggerTA(PuzzleEvents.SO_TimeAttack evt) {
        StartTimeAttack(timeLimit);
    }


    // 타임어택 시작
    private void StartTimeAttack(int timeLimit) {
        // 타임어택 시작
        isOn = true;
        nowTime = timeLimit;

        // 벽 활성화
        wall.SetActive(true);

        // 타임어택 이벤트 (사운드)

        // 시간표시
        timeGroup.alpha = 1f;
        UpdateTimer();
    }

    private void Update()
    {
        if (!isOn) return;

        // 타임어택 시작
        nowTime -= Time.deltaTime;

        if (nowTime <= 0)
        {
            // 타임어택 종료
            nowTime = 0f;
            isOn = false;

            EndTimeAttack();
        }

        UpdateTimer();
    }


    // 프레임마다 시간초 UI 업데이트
    private void UpdateTimer() {
        int sec = Mathf.FloorToInt(nowTime);    // 정수 초
        float cs = nowTime - sec;
        int csec = Mathf.FloorToInt(cs * 100f);

        if (secText != null) {
            secText.text = sec.ToString("00");  // 숫자 2자리 형식
        }
        if (csText != null) {
            csText.text = csec.ToString("00");  
        }
    }

    // 타임어택 종료
    private void EndTimeAttack() {
        // UI 타이머 삭제
        timeGroup.alpha = 0f;

        // 벽비활성화 타임라인
        StartCoroutine(FadeOutWall());
    }

    IEnumerator FadeOutWall() {
        wallTimeline.Play();

        yield return new WaitUntil(() => wallTimeline.state != PlayState.Playing);

        // 타임라인 끝나면 벽 실제 비활성화
        wall.SetActive(false);
    }

    //IEnumerator temp() {
    //    yield return new WaitForSeconds(25f);


    //    StartTimeAttack(timeLimit);
    //}
}
