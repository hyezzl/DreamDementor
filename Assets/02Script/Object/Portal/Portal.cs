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

    public GameObject player;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            // 다른 씬 이동시
            if (curScene != targetScene)
            {
                // 이벤트 발행
                EventBus.Instance.Publish<GameEvents.PortalSwitchScene>(new GameEvents.PortalSwitchScene(targetScene, spawnPoint));
            }
            // 같은 씬 내 이동
            else {
                if (player != null) { 
                    player.transform.position = spawnPoint;
                }
            }
        
        }
    }
}
