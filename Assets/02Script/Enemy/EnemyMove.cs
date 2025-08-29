using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Temp
/// </summary>

public class EnemyMove : MonoBehaviour, IMoveObject
{
    [SerializeField] private Transform player;

    [Header("Enemy Movement Setting")]
    [SerializeField] private float moveSpeed = 3f;
    // [SerializeField] private float chaseRange = 10f; 

    private Rigidbody rig;
    private EnemyController ec;
    private PlayerController pc;
    private NavMeshAgent na;
    private bool moveable;

    private void Awake()
    {
        if (!TryGetComponent<Rigidbody>(out rig))
        {
            Debug.Log("EnemyMove - Failed to Load Rigidbody");
        }
        if (!TryGetComponent<EnemyController>(out ec))
        {
            Debug.Log("EnemyMove - Failed to Load EnemyController");
        }
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("EnemyMove - Failed to Load PlayerController");
        if (!TryGetComponent<NavMeshAgent>(out na)) {
            Debug.Log("EnemyMove - Failed to Load NavMeshAgent");
        }
    }

    //임시
    private void Start()
    {
        ec.EnemyState = EnemyState.Chase;
    }

    private void OnEnable()
    {
        //EventBus.Instance.Subscribe<GameEvents.EnemyStateChange>(OnChange);
    }
    private void OnDisable()
    {
        //EventBus.Instance.Unsubscribe<GameEvents.EnemyStateChange>(OnChange);
    }

    private void OnChange(GameEvents.EnemyStateChange evt)
    {
        // 정지 상태에서 벗어남
        ec.EnemyState = evt.state;
    }

    private void FixedUpdate()
    {
        if(moveable)
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

    private void AIChasePlayer() {
        if (ec.EnemyState == EnemyState.Chase)
        {
            na.SetDestination(player.position);
        }
    }

    public void StopGame()
    {
        moveable = false;
    }

    public void ResumeGame()
    {
        moveable = true;
    }
}
