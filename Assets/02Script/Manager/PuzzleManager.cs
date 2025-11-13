using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PuzzleType
{ 
    Happy_Door,
    SorrowOne_LineToLine,
    SorrowOne_Lockpick,
    Sorrow_Door,

}


/// <summary>
/// PuzzleUI (Canvas) 통합관리
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    public RectTransform[] puzzles;
    public Button[] exitBTNs;
    public GameObject background;
    private PlayerController pc;
    private Canvas canvas;

    private int originSortOrder = 0;
    private Vector2 hidePos = new Vector2(0, -1080f);
    private Vector2 appearPos = Vector2.zero;
    private int curPuzzle = -1;         // 현재 열려있는 퍼즐

    private void Start()
    {
        // 초기화
        foreach (var puzzle in puzzles) { puzzle.gameObject.SetActive(false); }
        background.SetActive(false);
    }

    private void OnEnable()
    {
        if (!TryGetComponent<Canvas>(out canvas)) Debug.Log("PuzzleManager - Failed to Load Canvas");

        EventBus.Instance.Subscribe<UIEvents.OpenPuzzle>(OnOpenPuzzleUI);

        //exitBTN.onClick.AddListener(CloseAllPuzzleUI);
        foreach (var btn in exitBTNs) {
            btn.onClick.AddListener(CloseAllPuzzleUI);
        }
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.OpenPuzzle>(OnOpenPuzzleUI);

        //exitBTN.onClick.RemoveListener(CloseAllPuzzleUI);
        foreach (var btn in exitBTNs)
        {
            btn.onClick.RemoveListener(CloseAllPuzzleUI);
        }
    }

    private void OnOpenPuzzleUI(UIEvents.OpenPuzzle evt) {
        // 캔버스 우선순위 올림
        canvas.sortingOrder = 100;
        
        // 게임 모드 변경
        PlayerController.Instance.CurMode = GameMode.UIPuzzleMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.UIPuzzleMode));

        // 씬에 따른 다른 캔버스 Active
        switch (evt.puzzle) 
        {
            case PuzzleType.Happy_Door:
                OpenPuzzleUI(0);
                curPuzzle = 0;
                break;

            case PuzzleType.SorrowOne_LineToLine:
                OpenPuzzleUI(1);
                curPuzzle = 1;
                break;

            case PuzzleType.SorrowOne_Lockpick:
                OpenPuzzleUI(2);
                curPuzzle = 2;
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
            puzzle.gameObject.SetActive(false);
        }
        background.SetActive(true);
        puzzles[idx].gameObject.SetActive(true);
        UIAnim(puzzles[idx], true);
    }


    // 버튼
    public void CloseAllPuzzleUI() 
    {
        StartCoroutine(CloseUI());
        
        // 소팅오더 복원
        canvas.sortingOrder = originSortOrder; // 원래 순서로 복원
        curPuzzle = -1;
    }

    // true : Open / false : Close
    private void UIAnim(RectTransform puzzle, bool isOpen) {
        Vector2 targetPos = isOpen ? appearPos : hidePos;
        puzzle.DOAnchorPos(targetPos, 1f).SetEase(Ease.OutQuart);
    }

    private IEnumerator CloseUI() {
        if (curPuzzle < 0 || curPuzzle >= puzzles.Length) yield break;

        UIAnim(puzzles[curPuzzle], false);

        yield return new WaitForSeconds(0.5f);

        background.SetActive(false);
        foreach (var puzzle in puzzles)
        {
            puzzle.gameObject.SetActive(false);
        }

        // 게임모드 변경
        PlayerController.Instance.CurMode = GameMode.InspectMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
    }
}
