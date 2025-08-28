


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
}