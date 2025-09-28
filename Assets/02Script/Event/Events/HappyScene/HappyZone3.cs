using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyZone3 : TriggerZone
{
    // юс╫ц
    public string illID;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnTrigger(GameObject actor)
    {
        base.OnTrigger(actor);

        if (!isContacted) {
            EventBus.Instance.Publish<UIEvents.SceneCover>(new UIEvents.SceneCover(illID));
        }
    }

    public override void OnEndDialog(UIEvents.EndDialog evt)
    {
        base.OnEndDialog(evt);

        if (evt.eventID == eventID) { 
            EventBus.Instance.Publish<UIEvents.SceneDiscover>(new UIEvents.SceneDiscover());
        }
    }
}
