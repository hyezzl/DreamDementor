using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStep : MonoBehaviour
{
    public int curStep;     // 현재 단계 (0,1,2)

    [Header("Area 1: Room Loop")]
    public string[] roomEventIDs;
    private Dictionary<int, DialogData>[] roomDialogs;


    [Header("Light")]
    [SerializeField] private Light directional;     // 빛
    private float[] lightInt = { 0.99f, 0.66f, 0.33f };   // 스텝별 빛의 세기

    [Header("Step Objects")]
    [SerializeField] private GameObject[] stepObjs;


    private Vector3 warpPos = new Vector3(-8f, 0f, 0.15f);
    private IDatabase cacheDB;  // db캐싱


    public void InitData(IDatabase db)
    {
        cacheDB = db;

        if (roomEventIDs != null && roomEventIDs.Length > 0)
        {
            roomDialogs = new Dictionary<int, DialogData>[roomEventIDs.Length];
            for (int i = 0; i < roomEventIDs.Length; i++)
            {
                roomDialogs[i] = db.GetDialog(roomEventIDs[i], roomEventIDs[i]);
            }
        }
    }


    // 다음 방으로 이동 (외부?)
    public void ToNextStep(GameObject player) {
        StartCoroutine(WarpRoutine(player));
    }



    private IEnumerator WarpRoutine(GameObject player)
    {
        // 움직임 방지
        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        // 1. 화면 가리기 연출 시작 

        // 2. 0.8초 대기 (효과가 화면을 완전히 가릴 때까지)
        yield return new WaitForSeconds(0.8f);

        // 3. 단계 상승 및 룸 업데이트
        if (curStep < 2)
        {
            curStep++;
            UpdateRoom();
        }

        // 4. 플레이어 실제 워프
        WarpPlayer(player);

        // 5. 화면 다시 보여주기 연출


        // 6. 이벤트 발행
        if (roomDialogs != null && curStep < roomDialogs.Length)
        {
            var dialog = roomDialogs[curStep];
            var eventID = roomEventIDs[curStep];

            if (dialog != null)
            {
                EventBus.Instance.Publish<UIEvents.OpenDialog>(
                    new UIEvents.OpenDialog(eventID, dialog, GameMode.InspectMode));
            }
        }
    }


    private void UpdateRoom()
    {
        if (directional != null) directional.intensity = lightInt[curStep];

        for (int i = 0; i < stepObjs.Length; i++)
        {
            if (stepObjs[i] == null) continue;
            bool isActive = (i == curStep);     // 활성화 대상인지?

            if (isActive && !stepObjs[i].activeSelf)
            {
                // 부모(stepObjs[i]) 아래에 있는 모든 NPC 컴포넌트를 찾음
                NPC[] npcsInArea = stepObjs[i].GetComponentsInChildren<NPC>(true);

                foreach (var npc in npcsInArea)
                {
                    npc.Init(cacheDB);
                }
            }

            // 3. 이제 켬
            stepObjs[i].SetActive(isActive);
        }
    }



    private void WarpPlayer(GameObject player) {
        CharacterController cc = player.GetComponent<CharacterController>();
        PlayerMove pm = player.GetComponent<PlayerMove>();

        if (cc != null) cc.enabled = false;

        player.transform.position = warpPos;

        if (cc != null) cc.enabled = true;

        if (pm != null) { 
            pm.SetPlayerDir(Vector3.right);
        }
    }


}
