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
        Debug.Log($"상호작용이 왜 되냐고.. 누구세요ㅠ?? {gameObject.name}");

        string fullPath = GetGameObjectPath(gameObject);
        Debug.Log($"!!!!!!!!!!!!! \n경로: <color=cyan>{fullPath}</color> \n창문번호: {boardNum}F");

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
