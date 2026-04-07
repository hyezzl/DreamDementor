using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LineToLinePuzzle : MonoBehaviour
{
    public List<LineDropZone> zones;
    private PuzzleManager pm;

    private void Awake()
    {
        pm = transform.parent.GetComponent<PuzzleManager>();
        if (pm == null) Debug.Log("LineToLinePuzzle - failed to Load PuzzleManager");
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PuzzleEvents.SO_ConnectLine>(OnConnectLine);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PuzzleEvents.SO_ConnectLine>(OnConnectLine);
    }

    private void OnConnectLine(PuzzleEvents.SO_ConnectLine evt) {
        foreach (var zone in zones) {
            if (!zone.IsConnect) return;
        }
        Debug.Log("전선 연결 모두 완료!!!");

        // 전선 연결 모두 완료!!!
        EventBus.Instance.Publish<PuzzleEvents.SO_GetCoin>(new PuzzleEvents.SO_GetCoin());

        // 효과음

        // UI자동 close
        pm.CloseAllPuzzleUI();

        // 불 들어옴
        EventBus.Instance.Publish<PuzzleEvents.PassMinigame>(new PuzzleEvents.PassMinigame(PuzzleType.SorrowOne_LineToLine));

        // 완료 이후, 오브젝트와 상호작용 불가능하도록
        EventBus.Instance.Publish<PuzzleEvents.CutOff>(new PuzzleEvents.CutOff(60001001));
    }

}
