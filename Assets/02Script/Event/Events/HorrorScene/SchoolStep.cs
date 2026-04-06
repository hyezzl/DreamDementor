using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolStep : HorrorBaseStep
{
    public int curSchoolStep = 0;

    [Header("Area 3: School Loop")]
    public string[] schoolEventIDs;
    private Dictionary<int, DialogData>[] schoolDialogs;

    [Header("Step Objects")]
    [SerializeField] private GameObject[] stepObjs;

    [Header("Start Event")]
    public string schoolStartEventID;
    private Dictionary<int, DialogData> schoolStartDialog;

    private Vector3 warpPos = new Vector3(315.172394f, -0.783199549f, 4.47506618f); // 학교 시작지점

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

    public void ToNextStep(GameObject player)
    {
        StartCoroutine(WarpSequence(player, warpPos, Vector3.right));
    }

    protected override void ToNextMap()
    {
        if (curSchoolStep < 2)
        {
            curSchoolStep++;
        }
    }

    protected override void UpdateMap()
    {
        // 빛 조절
        //if (directional != null) directional.intensity = lightInt[curRoomStep];

        // 오브젝트 활성화 및 NPC 초기화
        for (int i = 0; i < stepObjs.Length; i++)
        {
            if (stepObjs[i] == null) continue;
            bool isActive = (i == curSchoolStep);

            // 활성화될 때 부모의 InitialzeNPC(공통 로직)를 사용하여 데이터 주입
            if (isActive && !stepObjs[i].activeSelf)
            {
                InitialzeNPC(stepObjs[i]);
            }

            stepObjs[i].SetActive(isActive);
        }
    }

    protected override IEnumerator AfterWarp()
    {
        yield return null;

        if (schoolDialogs != null && curSchoolStep < schoolDialogs.Length)
        {
            var dialog = schoolDialogs[curSchoolStep];
            var eventID = schoolEventIDs[curSchoolStep];

            if (dialog != null)
            {
                EventBus.Instance.Publish<UIEvents.OpenDialog>(
                    new UIEvents.OpenDialog(eventID, dialog, GameMode.InspectMode));
            }
        }
    }
}
