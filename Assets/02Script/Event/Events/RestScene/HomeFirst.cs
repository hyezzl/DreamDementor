using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeFirst : EventBase
{
    public string inPortalID = "P004";
    private bool isFirst = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.Instance.Subscribe<GameEvents.UsePortal>(OnPortal);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.Instance.Unsubscribe<GameEvents.UsePortal>(OnPortal);
    }

    private void OnPortal(GameEvents.UsePortal evt) {
        if (evt.portalID == inPortalID && !isFirst) {
            // E022
            EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(eventID));
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));
        }
    }
}
