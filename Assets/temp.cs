using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp : SceneStart
{
    protected override void Awake()
    {
        SwitchSceneManager.Instance.CurScene = SceneType.SorrowOneScene;
        base.Awake();
    }

    private void Start()
    {
        PlayerController.Instance.CurAspect = AspectMode.OnepersonMode;
        EventBus.Instance.Publish<GameEvents.AspectChange>(new GameEvents.AspectChange(AspectMode.OnepersonMode));
    }
}
