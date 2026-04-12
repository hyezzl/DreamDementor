using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


public enum CabinetDir
{
    Forward,
    Right,
}

public class Cabinet : CustomItem
{
    [Header("Pos")]
    private Transform inPos;     // 캐비닛 안에 있을 때 플레이어 좌표
    public Transform outPos;    // 캐비닛에서 나왔을 때 플레이어가 소환될 좌표

    [Header("Direction")]
    public CabinetDir outDir = CabinetDir.Forward;   // 캐비닛에서 나왔을 때 바라볼 방향

    [Header("State")]
    public bool isIn;       // 플레이어가 들어가있는지?
    public float exitDelay = 1.5f;    // 들어가고 다시 나올 수 있는 초
    public float reEnterDelay = 1.5f; // 밖에서 다시 들어갈 수 있게 되는 딜레이

    private float hideTime;     // 숨은 시점
    private float lastExitTime;  // 마지막으로 탈출한 시점


    private void Awake()
    {
        inPos = gameObject.transform;
    }

    public override void Interact()
    {
        // 이벤트 / 대화 중에는 캐비닛 나가기/들어오기 무시
        if (PlayerController.Instance.CurMode == GameMode.EventMode ||
            PlayerController.Instance.CurMode == GameMode.DialogMode)
        {
            return;
        }

        if (isIn || Time.time - lastExitTime < reEnterDelay)
        {
            Debug.Log("캐비닛: 재입장 대기 중...");
            return;
        }
        Enter();
    }

    private void Update()
    {
        if (isIn) {
            // 이벤트 / 대화 중에는 캐비닛 나가기/들어오기 무시
            //if (PlayerController.Instance.CurMode == GameMode.EventMode ||
            //    PlayerController.Instance.CurMode == GameMode.DialogMode)
            //{
            //    return;
            //}

            // 숨은지 2초가 지난상태에서, 스페이스바를 누르면 탈출
            if (Time.time - hideTime >= exitDelay)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Exit();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // 아직 2초 안지남
                    Debug.Log("캐비닛:아직 딜레이중");
                }
            }
        }
    }


    private void Enter() {
        isIn = true;
        hideTime = Time.time;

        //카메라

        // 플레이어에게 전달
        EventBus.Instance.Publish(new GameEvents.UseCabinet(true, inPos));

        // 괴물
    }

    private void Exit() {
        isIn = false;
        lastExitTime = Time.time;   // 탈출시간 기록

        // 카메라

        // 플레이어에게 전달
        EventBus.Instance.Publish(new GameEvents.UseCabinet(false, outPos, outDir));
    }
}
