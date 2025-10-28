using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class HappyOnePuzzles : EventBase
{
    public static int HOcurKeyCnt = 0;       // 현재 플레이어가 가진 열쇠 갯수
    public CameraFilterPack_FX_Glitch1 glitch;
    public CameraFilterPack_Colors_Adjust_PreFilters colorAdjust;

    [Header("Camera Ref")]
    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private CinemachineVirtualCamera enemyCam;

    [SerializeField] PlayableDirector appearEnemy;     // 귀신등장 타임라인

    // 임시로 카메라 ....
    private float lookEnemyDuration = 6f;


    protected override void OnEnable()
    {
        base.OnEnable();

        glitch = Camera.main.GetComponent<CameraFilterPack_FX_Glitch1>();
        if (glitch == null) Debug.Log("HappyOnePuzzles - Failed to Load CameraFilter");
        glitch.enabled = false;

        Debug.Log($"현재까지 모은 열쇠 갯수 : {HOcurKeyCnt}");
        EventBus.Instance.Subscribe<PuzzleEvents.HO_GetKey>(OnGetKey);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.Instance.Unsubscribe<PuzzleEvents.HO_GetKey>(OnGetKey);
    }

    private void OnGetKey(PuzzleEvents.HO_GetKey evt) {
        HOcurKeyCnt++;
        Debug.Log($"현재까지 모은 열쇠 갯수 : {HOcurKeyCnt}");

        ManageKeyCnt(HOcurKeyCnt, evt.npcID);
    }

    private void ManageKeyCnt(int curKeyCnt, string npcID) {
        if (curKeyCnt == 3) {
            // key개수가 3개에 도달하면 귀신 등장
            Debug.Log("열쇠 세개 모았으므로 괴물이 등장!");

            // EnemySpawn
            EventBus.Instance.Publish<PuzzleEvents.HO_AppearEnemy>(new PuzzleEvents.HO_AppearEnemy(npcID));

            // 이벤트 발행
            StartCoroutine(PlayEnemyEvent());
        }
    }


    private IEnumerator PlayEnemyEvent() {
        yield return new WaitForSeconds(2f);

        // 40번 이벤트 발행
        // 카메라 이동
        // 귀신 등장 브금
        //StartCoroutine(MoveCam());
        appearEnemy.Play();
        yield return new WaitUntil(() => appearEnemy.state != PlayState.Playing);

        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog));
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

            EventBus.Instance.Publish<GameEvents.FilterOn>(new GameEvents.FilterOn(FilterType.ColorAdjust, true));
            if (colorAdjust != null)
            {
                colorAdjust.enabled = true;
                colorAdjust.filterchoice = CameraFilterPack_Colors_Adjust_PreFilters.filters.RedWhite;
                colorAdjust.FadeFX = 0.5f;
            }
        }
        base.CloseDialog(evt);
    }

    // 카메라 이동 이벤트
    private IEnumerator MoveCam() {
        enemyCam.Priority = 100;

        yield return new WaitForSeconds(lookEnemyDuration);

        enemyCam.Priority = 1;
    }

}
