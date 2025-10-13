using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HappyOne : MonoBehaviour
{
    public string eventID = "E018";
    private IDatabase database;
    private Dictionary<int, DialogData> dialogs;

    void Start()
    {
        // 시작 시 1인칭 모드
        EventBus.Instance.Publish<GameEvents.AspectChange>(new GameEvents.AspectChange(AspectMode.OnepersonMode));

        //StartCoroutine(PlayIntro());
    }

    public void Init(IDatabase db)  // start시점에 실행
    {
        database = db;
        dialogs = database.GetDialog(eventID);
        if (dialogs == null) Debug.Log("IntroEvent - Failed to Load DialogData");

        StartCoroutine(PlayIntro());
    }


    private IEnumerator PlayIntro() {
        yield return null;

        // 맵 입장하자마자 이벤트 (대화 이벤트)
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(eventID));
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, dialogs));

    }

    

}
