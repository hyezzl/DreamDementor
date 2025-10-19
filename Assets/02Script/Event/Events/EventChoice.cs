using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventChoice : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.PlayChoiceEvent>(OccurChoice);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.PlayChoiceEvent>(OccurChoice);
    }

    private void OccurChoice(GameEvents.PlayChoiceEvent evt)
    {
        // eventID에 따른 데이터 찾아서 
    }
}
