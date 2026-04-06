using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public string portalID;     // P001

    [Header("location Info")]
    public SceneType curScene;
    public SceneType targetScene;
    public Vector3 spawnPoint;
    public Direction4 spawnDir;

    private GameObject player;
    private PlayerController pc;
    private PlayerMove pm;
    private CharacterController cc;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log($"{portalID} - Failed to Load PlayerController");

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.Log($"{portalID} - Failed to Load player");

        pm = FindAnyObjectByType<PlayerMove>();
        if (pm == null) Debug.Log($"{portalID} - Failed to Load PlayerMove");

        cc = player.GetComponent<CharacterController>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            // 다른 씬 이동시
            if (curScene != targetScene)
            {
                // 다른 씬 이동 시 저장
                CustomSave();

                // 이벤트 발행
                EventBus.Instance.Publish<GameEvents.PortalSwitchScene>(new GameEvents.PortalSwitchScene(curScene,targetScene, spawnPoint, spawnDir));
            }

            // 같은 씬 내 이동
            else {
                StartCoroutine(WarpInSameScene());
            }
        }
    }

    protected IEnumerator WarpInSameScene() {
        if (player == null) yield break;

        // 조작 방지 및 페이드 아웃 시작
        if (pc != null) pc.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        // 장면 전환
        // EventBus.Instance.Publish<UIEvents.FadeOut>(new UIEvents.FadeOut(0.5f));
        yield return new WaitForSeconds(0.5f);

        // 실제 워프
        if (cc != null) cc.enabled = false;

        player.transform.position = spawnPoint;
        EventBus.Instance.Publish<GameEvents.ForceDir>(new GameEvents.ForceDir(spawnDir));

        if (cc != null) cc.enabled = true;

        // 짧은 대기 (워프 후 카메라 위치 동기화 시간 확보)
        yield return new WaitForSeconds(0.1f);

        // 포탈 이벤트 발행 (대화 등이 이어질 수 있도록)
        EventBus.Instance.Publish<GameEvents.UsePortal>(new GameEvents.UsePortal(portalID));
    }

    protected virtual void CustomSave() { }
}
