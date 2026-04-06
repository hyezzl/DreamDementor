using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorrorFirst : SceneStart
{
    PlayerMove pm;

    [Header("Each Manager")]
    public RoomStep roomM;
    public StreetStep streetM;
    public SchoolStep schoolM;

    protected override void Awake()
    {
        SwitchSceneManager.Instance.CurScene = SceneType.HorrorScene;
        base.Awake();
    }

    public override void Init(IDatabase db)
    {
        base.Init(db);

        // 각 맵의 매니저들 데이터로드
        if (roomM != null) roomM.InitData(db);
        if (streetM != null) streetM.InitData(db);
        if (schoolM != null) schoolM.InitData(db);


        // 플레이어 오른쪽방향 향함
        PlayerMove pm = FindAnyObjectByType<PlayerMove>();
        if (pm != null)
        {
            pm.SetPlayerDir(Vector2.right);
        }

        // 기존 효과음 삭제
        EventBus.Instance.Publish<GameEvents.StopSFX>(new GameEvents.StopSFX());

        // 시작 시 3인칭 모드
        PlayerController.Instance.CurAspect = AspectMode.ThirdpersonMode;
        EventBus.Instance.Publish<GameEvents.AspectChange>(new GameEvents.AspectChange(AspectMode.ThirdpersonMode));

        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        SaveManager.Instance.SaveGame();
    }
    protected override void OnSceneStart()
    {
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(startEventID));
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(startEventID, initialDialog, GameMode.InspectMode));
    }

    protected override void AfterDialog(UIEvents.EndDialog evt)
    {
        if (evt.eventID == startEventID)
        {
            // 첫 대화 이벤트 끝난 후
            // 미니퀘스트 활성화
        }
    }
   
}
