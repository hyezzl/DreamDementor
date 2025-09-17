using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private SceneType curscene;

    private PlayerController pc;
    private CharacterController cc;
    private bool isDead = false;  // 플레이어 쥬금



    private void Awake()
    {
        if (!TryGetComponent<CharacterController>(out cc))
            Debug.Log("GameOver - Failed to Load CharacterController");
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("GameOver - Failed to Load PlayerController");
    }


    /// 이함수는 characterController가 이동할때만 작동 -> Enemy에서 충돌처리
    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    if (isDead) return;

    //    if (hit.gameObject.CompareTag("Enemy")) {
    //        Debug.Log("게임오버");
    //        isDead = true;
    //        EventBus.Instance.Publish<GameEvents.GameOver>(new GameEvents.GameOver(curscene, DeathType.CrashEnemy));

    //        // 게임모드
    //        pc.CurMode = GameMode.GameOverMode;
    //        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.GameOverMode));
    //    }
    //}


}
