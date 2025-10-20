using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HappyOne1 : EventBase
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(eventID));
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog));
        }
    }

    //public override void Init(IDatabase db)  // start시점에 실행
    //{
    //    base.Init(db);
    //}


}
