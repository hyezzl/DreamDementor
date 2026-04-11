using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol, // 순찰
    Chase,  // 플레이어 쫓는중
    LostTarget,     // 플레이어 쫓는 도중 어그로 풀림
    Stop,
    Masked, // 엄마로 분장 중
}

/// <summary>
/// 적의 상태 관리
/// </summary>
public class EnemyController : MonoBehaviour
{
    private EnemyState curState;
    public EnemyState CurEnemyState => curState;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.EnemyStateChange>(OnChangeEnemyState);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.EnemyStateChange>(OnChangeEnemyState);
    }


    private void OnChangeEnemyState(GameEvents.EnemyStateChange evt) {
        curState = evt.state;

        // 하위 Enemy관련 로직에 알림
        EventBus.Instance.Publish<GameEvents.UpdateEnemy>(new GameEvents.UpdateEnemy());
    }
}