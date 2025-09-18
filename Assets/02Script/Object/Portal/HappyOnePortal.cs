using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyOnePortal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventBus.Instance.Publish<GameEvents.SwitchScene>(new GameEvents.SwitchScene(SceneType.HappyScene));
        }
    }
}
