using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockpickPuzzle : MonoBehaviour
{
    public List<DragPin> pins;
    private PuzzleManager pm;

    private void Awake()
    {
        pm = transform.parent.GetComponent<PuzzleManager>();
        if (pm == null) Debug.Log("LineToLinePuzzle - failed to Load PuzzleManager");
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PuzzleEvents.SO_FixPin>(OnFixedPin);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PuzzleEvents.SO_FixPin>(OnFixedPin);
    }

    private void OnFixedPin(PuzzleEvents.SO_FixPin evt) {
        // 6개의 핀이 모두 fix됐을경우
        foreach (var pin in pins) {
            if (!pin.IsFixed) return;
        }

        Debug.Log("자물쇠 열기 완료!");

        // 효과음

        // 불들어옴

        pm.CloseAllPuzzleUI();

        // 상호작용 불가능 하도록
        EventBus.Instance.Publish<PuzzleEvents.CutOff>(new PuzzleEvents.CutOff(60001002));
    }
}
