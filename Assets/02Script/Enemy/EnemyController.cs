


public enum EnemyState
{
    Idle,
    Patrol, // ผ๘ย๛
    Chase,  // วรทนภฬพ๎ ยัดยม฿
    Stop,
}

public class EnemyController : Singleton<EnemyController>
{
    public EnemyState EnemyState { get; set; } = EnemyState.Stop;
}