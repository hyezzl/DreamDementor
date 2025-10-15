using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public string portalID;     // P001

    [Header("location Info")]
    public SceneType targetScene;
    public Vector3 spawnPoint;

    public GameObject player;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            Debug.Log("Ãæµ¹");
            //EventBus.Instance.Publish<GameEvents.PortalSwitchScene>(new GameEvents.PortalSwitchScene(targetScene, spawnPoint));
        
            player.transform.position = spawnPoint;
        }
    }
}
