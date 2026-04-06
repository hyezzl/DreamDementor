using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStep : HorrorBaseStep
{
    public int curRoomStep;     // 현재 단계 (0,1,2)

    [Header("Area 1: Room Loop")]
    public string[] roomEventIDs;
    private Dictionary<int, DialogData>[] roomDialogs;


    [Header("Light")]
    [SerializeField] private Light directional;     // 빛
    private float[] lightInt = { 0.99f, 0.66f, 0.33f };   // 스텝별 빛의 세기

    [Header("Step Objects")]
    [SerializeField] private GameObject[] stepObjs;


    private Vector3 warpPos = new Vector3(-8f, 0f, 0.15f);


    public override void InitData(IDatabase db)
    {
        base.InitData(db);

        if (roomEventIDs != null && roomEventIDs.Length > 0)
        {
            roomDialogs = new Dictionary<int, DialogData>[roomEventIDs.Length];
            for (int i = 0; i < roomEventIDs.Length; i++)
            {
                roomDialogs[i] = db.GetDialog(roomEventIDs[i], roomEventIDs[i]);
            }
        }
    }

    public void ToNextStep(GameObject player) {
        StartCoroutine(WarpSequence(player, warpPos, Vector3.right));
    }

    protected override void ToNextMap()
    {
        if (curRoomStep < 2)
        {
            curRoomStep++;
        }
    }

    protected override void UpdateMap()
    {
        // 빛 조절
        if (directional != null) directional.intensity = lightInt[curRoomStep];

        // 오브젝트 활성화 및 NPC 초기화
        for (int i = 0; i < stepObjs.Length; i++)
        {
            if (stepObjs[i] == null) continue;
            bool isActive = (i == curRoomStep);

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

        if (roomDialogs != null && curRoomStep < roomDialogs.Length)
        {
            var dialog = roomDialogs[curRoomStep];
            var eventID = roomEventIDs[curRoomStep];

            if (dialog != null)
            {
                EventBus.Instance.Publish<UIEvents.OpenDialog>(
                    new UIEvents.OpenDialog(eventID, dialog, GameMode.InspectMode));
            }
        }
    }
}
