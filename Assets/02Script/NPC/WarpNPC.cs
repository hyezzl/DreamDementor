using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpNPC : NPC
{
    [Header("Warp Settings")]
    [SerializeField] private bool isSceneChange = true; // 씬 이동 여부
    [SerializeField] private SceneType targetScene;     // 이동할 씬
    [SerializeField] private Vector3 spawnPoint;        // 워프 좌표
    [SerializeField] private Direction4 spawnDir;       // 워프 후 바라볼 방향
    [SerializeField] private string tempPortalID;

    private GameObject player;
    private CharacterController cc;

    protected override void Awake()
    {
        base.Awake();

        // 태그로 플레이어 오브젝트 찾음
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            cc = player.GetComponent<CharacterController>();
        }
        else { Debug.Log("WarpNPC - Failed to Load Player"); }
    }


    protected override void OnEndNpcDialog(UIEvents.EndNpcDialog evt)
    {
        base.OnEndNpcDialog(evt);
        Debug.Log("111111");

        // 대화가 완전히 끝났을 때만 실행
        if (evt.npcID == npcID)
        {
            Debug.Log("2222");

            StartCoroutine(WarpSequence());
        }
    }

    private IEnumerator WarpSequence()
    {
        // 1. 씬이동 연출?
        // EventBus.Instance.Publish<GameEvents.FadeOut>(new GameEvents.FadeOut(0.5f));

        pc.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        // 연출 대기 시간 (필요시)
        yield return new WaitForSeconds(0.5f);

        if (isSceneChange)
        {
            // 2. 다른 씬으로 이동할 경우
            SceneType curScene = SwitchSceneManager.Instance.CurScene;
            EventBus.Instance.Publish<GameEvents.PortalSwitchScene>(
                new GameEvents.PortalSwitchScene(curScene, targetScene, spawnPoint, spawnDir));
        }
        else
        {
            // 3. 같은 씬 내 위치 이동일 경우
            WarpInSameScene();
        }
    }

    private void WarpInSameScene()
    {
        if (cc != null) cc.enabled = false;

        if (player != null)
        {
            player.transform.position = spawnPoint;

            PlayerMove pm = player.GetComponent<PlayerMove>();
            if (pm != null) pm.SetPlayerDir(ConvertDir(spawnDir));
        }

        if (cc != null) cc.enabled = true;

        // 워프가 끝남을 알림
        EventBus.Instance.Publish<GameEvents.UsePortal>(new GameEvents.UsePortal(tempPortalID));
    }

    private Vector3 ConvertDir(Direction4 dir)
    {
        switch (dir)
        {
            case Direction4.Right: return Vector3.right;
            case Direction4.Left: return Vector3.left;
            case Direction4.Forward: return Vector3.forward;
            case Direction4.Back: return Vector3.back;
            default: return Vector3.right;
        }
    }
}
