using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class HorrorEvents : EventBase
{
    protected override void PlayEvent(GameEvents.PlayEvent evt)
    {
        if (evt.eventID == eventID)
        {
            EventBus.Instance.Publish(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));
        }
    }
}
