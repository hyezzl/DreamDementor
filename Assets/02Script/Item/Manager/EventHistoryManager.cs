
using System.Collections.Generic;
using UnityEngine;
using static GameEvents;

/// <summary>
/// 해당 씬 내에서 끝난 이벤트 / 대화 데이터 보관
/// 씬 방문
/// </summary>
public class EventHistoryManager : Singleton<EventHistoryManager>
{
    // 실행된 이벤트 목록 관리
    private static List<string> completedEvents = new();  // 실행된 이벤트 저장

    // 씬 방문 기록
    private Dictionary<SceneType, bool> visitScene = new();

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.EndEvent>(RecordEvent);
        EventBus.Instance.Subscribe<UIEvents.EndDialog>(RecordDialog);
        EventBus.Instance.Subscribe<UIEvents.EndNpcDialog>(RecordNpcDialog);
        EventBus.Instance.Subscribe<GameEvents.GameOver>(OnGameOver);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.EndEvent>(RecordEvent);
        EventBus.Instance.Unsubscribe<UIEvents.EndDialog>(RecordDialog);
        EventBus.Instance.Unsubscribe<UIEvents.EndNpcDialog>(RecordNpcDialog);
        EventBus.Instance.Unsubscribe<GameEvents.GameOver>(OnGameOver);
    }


    // 종료된 이벤트 기록
    public void RecordEvent(GameEvents.EndEvent evt) {
        if (!completedEvents.Exists(e => e == evt.eventID))
        {
            completedEvents.Add(evt.eventID);
            Debug.Log($"이벤트 기록 : {evt.eventID}");
        }
    }

    public void RecordDialog(UIEvents.EndDialog evt) {
        completedEvents.Add(evt.eventID);
        if (!completedEvents.Exists(e => e == evt.eventID))
        {
            completedEvents.Add(evt.eventID);
            Debug.Log($"이벤트 기록 : {evt.eventID}");
        }
    }

    public void RecordNpcDialog(UIEvents.EndNpcDialog evt) {
        completedEvents.Add(evt.npcID);
        if (!completedEvents.Exists(e => e == evt.npcID))
        {
            completedEvents.Add(evt.npcID);
            Debug.Log($"이벤트 기록 : {evt.npcID}");
        }
    }


    // 게임오버 시 죽은 씬 방문기록 초기화
    private void OnGameOver(GameEvents.GameOver evt)
    {
        // 죽은 씬 방문 기록 초기화 - 다시 그 씬 방문 시 첫방문 처리되도록
        ResetVisitSceneRecord(evt.scene);
    }



    // 외부호출 : 이벤트 완료 여부 확인 (+ Npc대화)
    public bool IsEventComplete(string eventID)
    {
        return completedEvents.Exists(e => e == eventID);
    }




    //=========================================================//

    // 씬 첫방문 여부 확인
    public bool isFirstVisit(SceneType scene) {
        if (visitScene.TryGetValue(scene, out bool isVisit)) {
            return !isVisit;
        }
        // 없으면 첫방문
        return true;
    }

    // 씬 방문 기록 추가
    public void RecordVisit(SceneType scene) {
        //Debug.Log($"{scene} 방문 기록!");
        visitScene[scene] = true;
    }

    // 씬 방문 기록 초기화 함수
    public void ResetVisitSceneRecord(SceneType scene)
    {
        if (visitScene.ContainsKey(scene))
        {
            visitScene[scene] = false; // 다시 첫 방문 상태로 설정
        }
    }


    // 내보내기
    public void LoadCompletedEvents(List<string> events)
    {
        if (events == null) return;
        completedEvents = new List<string>(events);
    }

    public void LoadVisitScenes(Dictionary<SceneType, bool> scenes)
    {
        if (scenes == null) return;
        visitScene = new Dictionary<SceneType, bool>(scenes);
    }
}
