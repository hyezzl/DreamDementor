using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetStep : MonoBehaviour
{
    [Header("Area 2: Street")]
    public string streetEventID;
    private Dictionary<int, DialogData> streetDialog;


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.UsePortal>(OnArriveMap);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.UsePortal>(OnArriveMap);
    }



    public void InitData(IDatabase db)
    {
        if (!string.IsNullOrEmpty(streetEventID))
            streetDialog = db.GetDialog(streetEventID, streetEventID);
    }


    private void OnArriveMap(GameEvents.UsePortal evt) {
        if (evt.portalID == "P009") {
            // 첫이벤트 실행
            EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(streetEventID));
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(streetEventID, streetDialog, GameMode.InspectMode));
        }
    }
}
