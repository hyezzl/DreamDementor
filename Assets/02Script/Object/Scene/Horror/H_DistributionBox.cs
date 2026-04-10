using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleEvents;

public class H_DistributionBox : CustomItem
{
    [SerializeField] private PuzzleType type;
    [SerializeField] private GameObject lights;

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

    private void Start()
    {
        if (lights != null)
            lights.SetActive(false);
    }

    public override void Interact()
    {
        //base.Interact();
        EventBus.Instance.Publish<UIEvents.OpenPuzzle>(new UIEvents.OpenPuzzle(type));
    }

    public void OnPassGame(PuzzleEvents.PassMinigame evt)
    {
        if (evt.puzzle == type)
        {
            // 미니게임 패스 시 불
            if (lights != null)
                lights.SetActive(true);

            // 타임라인2 재생
        }
    }
}
