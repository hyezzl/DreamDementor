using UnityEngine;

//public enum EventType
//{ 
//    Narration,
//    Cutscene,
//    Conversation,  // only 대화 (플레이어의 입력 포함)
//    SceneChange,
//}


public class EventFlowManager : Singleton<EventFlowManager> // 싱글톤일 이유가?
{

    protected override void DoAwake()
    {
        base.DoAwake();
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.PlayEvent>(OccurEvent);
        EventBus.Instance.Subscribe<GameEvents.PlayChoiceEvent>(OccurChoice);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.PlayEvent>(OccurEvent);
        EventBus.Instance.Unsubscribe<GameEvents.PlayChoiceEvent>(OccurChoice);
    }


    private void OccurEvent(GameEvents.PlayEvent evt) { 
        
    }

    private void OccurChoice(GameEvents.PlayChoiceEvent evt) { 
        // eventID에 따른 데이터 찾아서 
    }
}


