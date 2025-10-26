using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HappyPuzzleManager : MonoBehaviour
{
    public List<DropZone> zones;    // 3개의 퍼즐
    public SceneType nextScene = SceneType.RestScene;     // 이동할 씬


    public void TouchDoorRing() {

        // 모든 드랍존이 정답인지?
        foreach (var zone in zones) {
            if (!zone.IsCorrect) {
                Debug.Log("퍼즐 미완성");
                // 효과음 (열리지 않는)
                return;
            }
        }
        // 씬이동
        EventBus.Instance.Publish<GameEvents.SwitchScene>(new GameEvents.SwitchScene(SceneType.RestScene));
    }
}
