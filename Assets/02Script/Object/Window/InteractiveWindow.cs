using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class InteractiveWindow : CustomItem
{
    [Header("Information")]
    [SerializeField] private HorrorFloor floor;

    [Header("isContact?")]
    [SerializeField] private bool isContact = false;

    [Header("Window Setting")]
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private CinemachineVirtualCamera windowCam;
    [SerializeField] private int boardNum;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite[] numbers;


    private bool isViewing = false;
    private bool isChange = false;      // 중복실행방지

    public override void Interact()
    {
        if (isChange || isViewing) return;
        if (PlayerController.Instance.CurMode == GameMode.DialogMode ||
        PlayerController.Instance.CurMode == GameMode.EventMode) return;

        StartCoroutine(ViewSequence());
    }

    private void Update()
    {
        if (isChange) return;

        //if (isViewing && Input.anyKeyDown)
        //{
        //    StartCoroutine(CloseWindowSequence());
        //}

        if (isViewing && Input.anyKeyDown)
        {
            if (PlayerController.Instance.CurMode == GameMode.UIPuzzleMode)
            {
                StartCoroutine(CloseWindowSequence());
            }
        }
        //if (isViewing)
        //{
        //    if (PlayerController.Instance.CurMode != GameMode.UIPuzzleMode)
        //    {
        //        // 시퀀스를 돌리지 않고 즉시 상태만 해제 (모드 복구 방지)
        //        isViewing = false;
        //        windowCam.Priority = 0;
        //        return;
        //    }

        //    // 정상적인 닫기 시도
        //    if (Input.anyKeyDown)
        //    {
        //        StartCoroutine(CloseWindowSequence());
        //    }
        //}
    }


    private void OpenWindowView() {
        StartCoroutine(ViewSequence());
    }

    IEnumerator ViewSequence()
    {
        if (!isContact) {
            // 처음에만 기록
            isContact = true;
            EventBus.Instance.Publish(new PuzzleEvents.H_CheckWindow(itemID, floor));
            Debug.Log($"{floor} Window {itemID} : 기록됨");
        } 

        isChange = true;
        isViewing = true;

        if (sr != null) sr.sprite = numbers[boardNum];

        windowCam.Priority = 100;

        PlayerController.Instance.CurMode = GameMode.UIPuzzleMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.UIPuzzleMode));

        // 대기
        yield return new WaitForSeconds(0.3f);
        isChange = false;
    }

    IEnumerator CloseWindowSequence()
    {
        isChange = true; // 입력을 즉시 차단
        isViewing = false;

        // 카메라 및 모드 복구
        windowCam.Priority = 0;
        PlayerController.Instance.CurMode = GameMode.InspectMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));

        yield return new WaitForSeconds(0.8f);

        isChange = false;
    }

    //temp
    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = obj.name + "/" + path;
        }
        return path;
    }
}
