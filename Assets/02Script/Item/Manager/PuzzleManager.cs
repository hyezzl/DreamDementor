using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 0 : Happy
// 1 : 


/// <summary>
/// PuzzleUI (Canvas) 통합관리
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    public GameObject[] puzzles;
    public Button exitBTN;
    private PlayerController pc;
    private Canvas canvas;

    private int originSortOrder = 0;

    private void OnEnable()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("PuzzleManager - Failed to Load PlayerController");
        if (!TryGetComponent<Canvas>(out canvas)) Debug.Log("PuzzleManager - Failed to Load Canvas");

        EventBus.Instance.Subscribe<UIEvents.OpenPuzzle>(OnOpenPuzzleUI);

        exitBTN.onClick.AddListener(CloseAllPuzzleUI);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.OpenPuzzle>(OnOpenPuzzleUI);

        exitBTN.onClick.RemoveListener(CloseAllPuzzleUI);
    }

    private void OnOpenPuzzleUI(UIEvents.OpenPuzzle evt) {
        // 캔버스 우선순위 올림
        canvas.sortingOrder = 1000;
        
        // 게임 모드 변경
        pc.CurMode = GameMode.UIPuzzleMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.UIPuzzleMode));

        // 씬에 따른 다른 캔버스 Active
        switch (evt.curScene) 
        {
            case SceneType.HappyScene:
                Debug.Log("해피 퍼즐 열림");
                OpenPuzzleUI(0);
                break;

            case SceneType.SorrowScene:
                Debug.Log("슬픔 퍼즐 열림");
                OpenPuzzleUI(1);
                break;

            default:
                Debug.Log("퍼즐 존재하지 않음");
                break;
        }
    }

    // 선택적 퍼즐UI 켜기
    private void OpenPuzzleUI(int idx) {
        foreach (var puzzle in puzzles)
        {
            puzzle.SetActive(false);
        }
        puzzles[idx].SetActive(true);
    }

    // 버튼
    public void CloseAllPuzzleUI() {
        foreach (var puzzle in puzzles)
        {
            puzzle.SetActive(false);
        }

        // 게임모드 변경
        pc.CurMode = GameMode.InspectMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));

        // 소팅오더 복원
        canvas.sortingOrder = originSortOrder; // 원래 순서로 복원
    }
}
