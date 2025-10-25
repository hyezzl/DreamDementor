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

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log($"{portalID} - Failed to Load PlayerController");

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.Log($"{portalID} - Failed to Load player");

        pm = FindAnyObjectByType<PlayerMove>();
        if (pm == null) Debug.Log($"{portalID} - Failed to Load PlayerMove");
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            // 다른 씬 이동시
            if (curScene != targetScene)
            {
                // 이벤트 발행
                EventBus.Instance.Publish<GameEvents.PortalSwitchScene>(new GameEvents.PortalSwitchScene(targetScene, spawnPoint, spawnDir));
            }

            // 같은 씬 내 이동
            else {
                if (player != null) {
                    // 강제이동
                    player.transform.position = spawnPoint;
                    EventBus.Instance.Publish<GameEvents.ForceDir>(new GameEvents.ForceDir(spawnDir));
                    
                    // 이벤트모드
                    pc.CurMode = GameMode.EventMode;
                    EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

                    // 포탈이벤트 발행
                    EventBus.Instance.Publish<GameEvents.UsePortal>(new GameEvents.UsePortal(portalID));
                }
            }
        }
    }
}
