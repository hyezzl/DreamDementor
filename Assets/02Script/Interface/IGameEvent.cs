using UnityEngine;

public interface IGameEvent
{
    void RecordEvent(GameEvents.EndEvent evt);  // 실행된 이벤트 기록
}
