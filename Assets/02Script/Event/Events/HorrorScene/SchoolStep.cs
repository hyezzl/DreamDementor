using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolStep : HorrorBaseStep
{
    public int curSchoolStep = 0;

    [Header("Area 3: School Loop")]
    public string[] schoolEventIDs;
    private Dictionary<int, DialogData>[] schoolDialogs;

    [Header("Start Event")]
    public string schoolStartEventID;
    private Dictionary<int, DialogData> schoolStartDialog;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.UsePortal>(OnArriveMap);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.UsePortal>(OnArriveMap);
    }


    public override void InitData(IDatabase db)
    {
        base.InitData(db);

        // 시작 이벤트
        if (!string.IsNullOrEmpty(schoolStartEventID))
            schoolStartDialog = db.GetDialog(schoolStartEventID, schoolStartEventID);

        // 루프 이벤트
        if (schoolEventIDs != null && schoolEventIDs.Length > 0)
        {
            schoolDialogs = new Dictionary<int, DialogData>[schoolEventIDs.Length];
            for (int i = 0; i < schoolEventIDs.Length; i++)
            {
                schoolDialogs[i] = db.GetDialog(schoolEventIDs[i], schoolEventIDs[i]);
            }
        }
    }

    private void OnArriveMap(GameEvents.UsePortal evt)
    {
        if (evt.portalID == "P010")
        {
            if (!string.IsNullOrEmpty(schoolStartEventID) && schoolStartDialog != null)
            {
                // 첫 이벤트 실행 기록
                EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(schoolStartEventID));

                EventBus.Instance.Publish<UIEvents.OpenDialog>(
                    new UIEvents.OpenDialog(schoolStartEventID, schoolStartDialog, GameMode.InspectMode));
            }
            else
            {
                Debug.LogWarning($"SchoolStep - Failed to Load DialogData");

                PlayerController.Instance.CurMode = GameMode.InspectMode;
                EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
            }
        }
    }
}
