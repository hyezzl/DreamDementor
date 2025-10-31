using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 튜토리얼에서 DeadScene 이후 대사 + 씬이동
/// </summary>

public class TutorialDead : EventBase
{
    protected override void PlayEvent(GameEvents.PlayEvent evt) {
        if (evt.eventID == this.eventID) {
            EventBus.Instance.Publish<UIEvents.OpenMonologue>(new UIEvents.OpenMonologue(eventID, initialDialog));
        }
    }

    protected override void CloseDialog(UIEvents.EndDialog evt) {
        base.CloseDialog(evt);

        if (evt.eventID == this.eventID) {
            // HappyScene으로 이동
            EventBus.Instance.Publish<GameEvents.SwitchScene>(new GameEvents.SwitchScene(SceneType.TutorialScene, SceneType.HappyScene));
        }
    }
}
