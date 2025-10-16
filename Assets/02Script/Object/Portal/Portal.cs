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

    public GameObject player;
    public PlayerController pc;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log($"{portalID} - Failed to Load PlayerController");
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
                    // todo : 이동 직후 로직 부족한듯
                    // 이벤트모드
                    pc.CurMode = GameMode.EventMode;
                    EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

                    player.transform.position = spawnPoint;
                    EventBus.Instance.Publish<GameEvents.ForceDir>(new GameEvents.ForceDir(spawnDir));
                }
            }
        }
    }
}
