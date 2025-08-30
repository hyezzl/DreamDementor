using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private SceneType curscene;

    private CharacterController cc;
    private bool isDead = false;  // 플레이어 쥬금



    private void Awake()
    {
        if (!TryGetComponent<CharacterController>(out cc))
            Debug.Log("GameOver - Failed to Load CharacterController");
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isDead) return;

        Debug.Log($"충돌 오브젝트 이름 : {hit.gameObject.name}");
        if (hit.gameObject.CompareTag("Enemy")) {
            Debug.Log("게임오버");
            isDead = true;
            EventBus.Instance.Publish<GameEvents.GameOver>(new GameEvents.GameOver(curscene, DeathType.CrashEnemy));
        }
    }
}
