using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorrorBaseStep : MonoBehaviour
{
    protected IDatabase cacheDB;

    public virtual void InitData(IDatabase db)
    {
        cacheDB = db;
    }

    // 공통 워프 로직
    protected IEnumerator WarpSequence(GameObject player, Vector3 targetPos, Vector3 lookDir)
    {
        // 1. 조작 방지
        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        // 2. 화면 가리기 연출 

        yield return new WaitForSeconds(0.8f);

        // 3. 중간 로직 실행
        ToNextMap();
        UpdateMap();

        // 4. 실제 위치 이동
        WarpPlayer(player, targetPos, lookDir);

        // 5. 화면 다시 보여주기

        yield return new WaitForSeconds(0.2f);

        // 6. 워프 종료 후 이벤트 발행
        yield return StartCoroutine(AfterWarp());
    }

    private void WarpPlayer(GameObject player, Vector3 pos, Vector3 dir) {
        CharacterController cc = player.GetComponent<CharacterController>();
        PlayerMove pm = player.GetComponent<PlayerMove>();

        if (cc != null) cc.enabled = false;
        player.transform.position = pos;
        if (cc != null) cc.enabled = true;

        if (pm != null) pm.SetPlayerDir(dir);
    }

    // NPC 자동 초기화
    protected virtual void InitialzeNPC(GameObject root) {
        if (root == null) return;

        NPC[] npcs = root.GetComponentsInChildren<NPC>(true);

        foreach (var npc in npcs)
        {
            npc.Init(cacheDB);
        }
    }


    // override용
    protected virtual void UpdateMap() { }

    protected virtual void ToNextMap() { }

    protected virtual IEnumerator AfterWarp() { yield return null; }
}
