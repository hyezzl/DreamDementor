using UnityEngine;

public class HappyPortal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            EventBus.Instance.Publish<GameEvents.SwitchScene>(new GameEvents.SwitchScene(SceneType.HappyOneScene));
        }
    }
}
