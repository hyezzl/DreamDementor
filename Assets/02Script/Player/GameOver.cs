using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private Scene curscene;

    private CharacterController cc;
    

    private void Awake()
    {
        if (!TryGetComponent<CharacterController>(out cc))
            Debug.Log("GameOver - Failed to Load CharacterController");
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log($"충돌 오브젝트 이름 : {hit.gameObject.name}");
        if (hit.gameObject.CompareTag("Enemy")) {
            Debug.Log("게임오버");
            EventBus.Instance.Publish<GameEvents.GameOver>(new GameEvents.GameOver(curscene, DeathType.CrashEnemy));
        }
    }
}
