using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCrash : MonoBehaviour
{
    [SerializeField] private SceneType curScene;

    private PlayerController pc;
    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("GameOver - Failed to Load PlayerController");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("게임오버! 플레이어 충돌");
            EventBus.Instance.Publish<GameEvents.GameOver>(new GameEvents.GameOver(curScene, DeathType.CrashEnemy));

            // 게임모드
            pc.CurMode = GameMode.GameOverMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.GameOverMode));
        }
    }
}
