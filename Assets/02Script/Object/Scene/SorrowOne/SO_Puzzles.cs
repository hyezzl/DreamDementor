using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SO_Puzzles : CustomItem
{
    [SerializeField] private PuzzleType type;
    [SerializeField] private GameObject lights;     // 라이트 + 숫자

    private InspectableData data;

    // 자판기 사용가능한지?
    private bool canInsert = false;

    public override void Init(IDatabase db) {
        if (itemID != 0) { 
            database = db;
            data = database.GetInspectable(itemID);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();

        EventBus.Instance.Subscribe<PuzzleEvents.SO_SetMachine>(OnSetMachine);
        EventBus.Instance.Subscribe<PuzzleEvents.PassMinigame>(OnPassGame);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        EventBus.Instance.Unsubscribe<PuzzleEvents.SO_SetMachine>(OnSetMachine);
        EventBus.Instance.Unsubscribe<PuzzleEvents.PassMinigame>(OnPassGame);
    }


    private void Start()
    {
        if(lights != null)
            lights.SetActive(false);
    }

    public override void Interact()
    {
        // 선행 조건이 되지않은 상태에서 자판기와 interact
        if (type == PuzzleType.SorrowOne_VendingMachine && !canInsert) {
            Debug.Log("선행 조건 완료전에는 자판기 열 수 없음");
            // monologue
            EventBus.Instance.Publish<UIEvents.ItemMonologue>(new UIEvents.ItemMonologue(itemID, data.monologue));

            return;
        }

        base.Interact();

        EventBus.Instance.Publish<UIEvents.OpenPuzzle>(new UIEvents.OpenPuzzle(type));
    }

    public void OnPassGame(PuzzleEvents.PassMinigame evt) {
        if (evt.puzzle == type) { 
            // 미니게임 패스 시 불
            if(lights != null)
                lights.SetActive(true);
        }
    }

    public void OnSetMachine(PuzzleEvents.SO_SetMachine evt) {
        canInsert = true;
    }
}
