


using UnityEditor;

public enum EnemyState
{
    Idle,
    Patrol, // 순찰
    Chase,  // 플레이어 쫓는중
    Stop,
    Masked, // 엄마로 분장 중
}

public class EnemyController : Singleton<EnemyController>
{
    public EnemyState EnemyState { get; set; } = EnemyState.Stop;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.SwitchScene>(DeactiveEnemy);
    }
    private void OnDisable()
    {
        EventBus.Instance.Subscribe<GameEvents.SwitchScene>(DeactiveEnemy);
    }

    // 씬변경 시 Enemy Deactive
    private void DeactiveEnemy(GameEvents.SwitchScene evt) { 
        this.gameObject.SetActive(false);
    }
}