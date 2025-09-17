
using System.Collections.Generic;

public class EventHistoryManager :Singleton<EventHistoryManager>
{
    // 실행된 이벤트 목록 관리
    private List<string> endedEvents = new();  // 실행된 이벤트 저장

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.PlayEvent>(RecordEvent);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.PlayEvent>(RecordEvent);
    }

    // 종료된 이벤트 기록
    public void RecordEvent(GameEvents.PlayEvent evt) {
        endedEvents.Add(evt.eventID);
    }

    // 외부 호출
    public IReadOnlyList<string> GetEndedEvents() {
        return endedEvents.AsReadOnly();
    }
}
