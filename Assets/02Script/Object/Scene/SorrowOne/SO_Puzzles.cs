using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_Puzzles : CustomItem
{
    [SerializeField] private PuzzleType type;
    [SerializeField] private GameObject lights;     // 라이트 + 숫자

    private void Start()
    {
        lights.SetActive(false);
    }

    public override void Interact()
    {
        base.Interact();

        EventBus.Instance.Publish<UIEvents.OpenPuzzle>(new UIEvents.OpenPuzzle(type));
    }

    public override void OnEnable()
    {
        base.OnEnable();
        EventBus.Instance.Subscribe<PuzzleEvents.PassMinigame>(OnPassGame);
    }
    public override void OnDisable()
    {
        base.OnDisable();
        EventBus.Instance.Unsubscribe<PuzzleEvents.PassMinigame>(OnPassGame);
    }

    public void OnPassGame(PuzzleEvents.PassMinigame evt) {
        if (evt.puzzle == type) { 
            // 미니게임 패스 시 불
            lights.SetActive(true);
        }
    }
}
