using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class HappyOnePuzzles : EventBase
{
    public static int HOcurKeyCnt = 0;       // 현재 플레이어가 가진 열쇠 갯수
    public CameraFilterPack_FX_Glitch1 glitch;
    public CameraFilterPack_Colors_Adjust_PreFilters colorAdjust;

    [Header("Camera Ref")]
    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private CinemachineVirtualCamera enemyCam;

    [SerializeField] PlayableDirector appearEnemy;     // 귀신등장 타임라인

    [SerializeField] GameObject hand;

    [SerializeField] GameObject blockzone;

    // 임시 퀘스트
    [SerializeField] private CanvasGroup questgroup;
    [SerializeField] private TextMeshProUGUI quest;



    protected override void OnEnable()
    {
        base.OnEnable();

        glitch = Camera.main.GetComponent<CameraFilterPack_FX_Glitch1>();
        if (glitch == null) Debug.Log("HappyOnePuzzles - Failed to Load CameraFilter");
        glitch.enabled = false;

        Debug.Log($"현재까지 모은 열쇠 갯수 : {HOcurKeyCnt}");
        EventBus.Instance.Subscribe<PuzzleEvents.HO_GetKey>(OnGetKey);
        EventBus.Instance.Subscribe<GameEvents.GameOver>(OnGameOver);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.Instance.Unsubscribe<PuzzleEvents.HO_GetKey>(OnGetKey);
        EventBus.Instance.Unsubscribe<GameEvents.GameOver>(OnGameOver);
    }

    private void OnGetKey(PuzzleEvents.HO_GetKey evt) {
        HOcurKeyCnt++;
        Debug.Log($"현재까지 모은 열쇠 갯수 : {HOcurKeyCnt}");

        UpdateQuest();
        ManageKeyCnt(HOcurKeyCnt, evt.npcID);
    }

    private void ManageKeyCnt(int curKeyCnt, string npcID) {

        if (curKeyCnt == 3) {
            // key개수가 3개에 도달하면 귀신 등장
            Debug.Log("열쇠 세개 모았으므로 괴물이 등장!");

            // 문을 막고있던 BlockZone 삭제
            blockzone.SetActive(false);
            Debug.Log("문을 막고 있던 블락존 삭제!");

            // EnemySpawn
            EventBus.Instance.Publish<PuzzleEvents.HO_AppearEnemy>(new PuzzleEvents.HO_AppearEnemy(npcID));

            // 이벤트 발행
            StartCoroutine(PlayEnemyEvent());
        }
    }


    private IEnumerator PlayEnemyEvent() {
        yield return new WaitForSeconds(2f);
        // 귀신 등장 브금
        EventBus.Instance.Publish<GameEvents.PlaySFX>(new GameEvents.PlaySFX(SFXType.grrr));

        // 이벤트 모드
        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        // 카메라이동 타임라인
        appearEnemy.Play();
        yield return new WaitUntil(() => appearEnemy.state != PlayState.Playing);

        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));
    }

    protected override void CloseDialog(UIEvents.EndDialog evt)
    {
        if (evt.eventID == eventID) {
            // 귀신 등장관련 이벤트(E40) 이후

            // 괴물 ChaseMode
            Debug.Log("술래잡기 시작!");
            EventBus.Instance.Publish<GameEvents.EnemyStateChange>(new GameEvents.EnemyStateChange(EnemyState.Chase));

            // 카메라효과
            EventBus.Instance.Publish<GameEvents.FilterOn>(new GameEvents.FilterOn(FilterType.Glitch, true));
            if (glitch != null) { 
                glitch.enabled = true;
                glitch.Glitch = 0.2f;
            }

            EventBus.Instance.Publish<GameEvents.FilterOn>(new GameEvents.FilterOn(FilterType.HorrorReverse, true));
            if (colorAdjust != null)
            {
                colorAdjust.enabled = true;
                colorAdjust.filterchoice = CameraFilterPack_Colors_Adjust_PreFilters.filters.RedWhite;
                colorAdjust.FadeFX = 0.5f;
            }

            // 퀘스트 갱신
            quest.text = "귀신을 피해 탈출하기";
        }
        base.CloseDialog(evt);
    }

    private void UpdateQuest() {
        string preText = "●  미로를 탐험하여 열쇠 찾기 ( ";
        string aftText = " / 3 )";

        quest.text = preText + HOcurKeyCnt.ToString() + aftText;
    }


    // 게임오버 시 실행
    private void OnGameOver(GameEvents.GameOver evt) {
        // 미니퀘스트 종료
        questgroup.alpha = 0f;

        // 필터효과 종료
        //EventBus.Instance.Publish<GameEvents.FilterOff>
        glitch.enabled = false;
        colorAdjust.enabled = false;

        // 임시? 열쇠개수 초기화
        HOcurKeyCnt = 0;

    }

}
