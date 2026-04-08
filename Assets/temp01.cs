using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp01 : InteractableItem
{
    // Start is called before the first frame update

    public override void Interact()
    {
        base.Interact();

        Debug.Log("200이벤트 실행완료");
        EventBus.Instance.Publish<GameEvents.EndEvent>(new GameEvents.EndEvent("E200"));
    }



}
