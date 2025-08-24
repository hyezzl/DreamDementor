using UnityEngine;

/// <summary>
/// Temp
/// </summary>

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("Enemy Movement Setting")]
    [SerializeField] private float moveSpeed = 3f;
    // [SerializeField] private float chaseRange = 10f; 

    private Rigidbody2D rig;
    private EnemyController ec;
    private PlayerController pc;

    private void Awake()
    {
        if (!TryGetComponent<Rigidbody2D>(out rig))
        {
            Debug.Log("EnemyMove - Failed to Load Rigidbody");
        }
        if (!TryGetComponent<EnemyController>(out ec))
        {
            Debug.Log("EnemyMove - Failed to Load EnemyController");
        }
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("EnemyMove - Failed to Load PlayerController");
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.EnemyStateChange>(OnChange);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.EnemyStateChange>(OnChange);
    }

    private void OnChange(GameEvents.EnemyStateChange evt)
    {
        // 정지 상태에서 벗어남
        ec.EnemyState = evt.state;
    }

    private void FixedUpdate()
    {
        ChasePlayer();

        Debug.Log("플레이어 위치: " + player.transform.position);
        Debug.Log("적 위치: " + transform.position);

        float dist = Vector3.Distance(player.transform.position, transform.position);
        Debug.Log("두 거리: " + dist);
    }


    private void ChasePlayer()
    {
        // 조건
        if (ec.EnemyState == EnemyState.Chase)
        {

            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= 100f) // 임시
            {
                Vector2 dir = (player.position - transform.position).normalized;
                rig.velocity = dir * moveSpeed;
            }
            else
            {
                rig.velocity = Vector2.zero;
            }
        }
    }
}
