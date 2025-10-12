using UnityEngine;

public enum EventType
{ 
    Narration,
    Cutscene,
    Conversation,  // only 대화 (플레이어의 입력 포함)
    SceneChange,
}


public class EventFlowManager : Singleton<EventFlowManager>
{
    // 이벤트 연결
    private IntroEvent intro;

    // 아니면 이벤트 동적생성해서 Dictionary관리?
    //private Dictionary<string, IGameEvent> gameEvents;


    protected override void DoAwake()
    {
        base.DoAwake();
        if (!TryGetComponent<IntroEvent>(out intro)) Debug.Log("EventFlowManager - Failed to Load IntroEvent");

        //아니면,
        //gameEvents.Add("Intro", new IntroEvent());
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.PlayEvent>(OccurEvent);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.PlayEvent>(OccurEvent);
    }


    private void OccurEvent(GameEvents.PlayEvent evt) { 
        
    }
}


