using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class InteractiveWindow : CustomItem
{
    [Header("Window Setting")]
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private CinemachineVirtualCamera windowCam;
    [SerializeField] private int boardNum;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite[] numbers;


    private bool isViewing = false;

    public override void Interact()
    {
        if (!isViewing)
        {
            OpenWindowView();
        }
        else {
            CloseWindowView();
        }
    }

    private void Update()
    {
        // 보는 도중 ESC 누르면 종료
        if (isViewing && Input.GetKeyDown(KeyCode.Escape)) {
            CloseWindowView();
        }
        //if (isViewing && Input.GetKeyDown(KeyCode.Space))
        //{
        //    CloseWindowView();
        //}
    }


    private void OpenWindowView() {
        // 윈도우 캠으로 옮겨감
        isViewing = true;

        // 칠판 숫자 업데이트
        if (sr != null) { 
            sr.sprite = numbers[boardNum];
        }

        // 카메라 변경 + 블렌드x
        //if (brain != null)
        //{
        //    // 원래 설정되어 있던 블렌드 저장
        //    var originalBlend = brain.m_DefaultBlend;

        //    // Cut 변경
        //    brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);

        //    // 카메라 전환
        //    windowCam.Priority = 100;

        //    // 복구
        //    brain.m_DefaultBlend = originalBlend;
        //}
        windowCam.Priority = 100;

        // 모드변경
        PlayerController.Instance.CurMode = GameMode.UIPuzzleMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.UIPuzzleMode));
    }

    private void CloseWindowView() {
        isViewing = false;

        // 카메라 이동
        windowCam.Priority = 0;

        PlayerController.Instance.CurMode = GameMode.InspectMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
    }
}
