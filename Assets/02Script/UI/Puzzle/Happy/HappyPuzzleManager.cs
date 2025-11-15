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
        // 스테이지 종료 알림
        EventBus.Instance.Publish<GameEvents.StageEnd>(new GameEvents.StageEnd(Stage.Happy));

        // 사용된 3개 아이템 제거
        InventoryManager.Instance.RemoveItem(10001002);
        InventoryManager.Instance.RemoveItem(10001003);
        InventoryManager.Instance.RemoveItem(10001004);

        // 씬이동
        EventBus.Instance.Publish<GameEvents.SwitchScene>(new GameEvents.SwitchScene(SceneType.HappyScene, SceneType.RestScene));
    }
}
