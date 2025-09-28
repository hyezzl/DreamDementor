using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour, IMoveObject
{
    [SerializeField] private Transform player;
    [SerializeField] private Animator anim;
    public EnemyState curState;
    public bool moveable;

    [Header("Enemy Movement Setting")]
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody rig;
    private EnemyController ec;
    private PlayerController pc;
    private NavMeshAgent na;
    //private bool moveable = true;
    //private EnemyState curState;

    // 상태값
    private bool isMonster = false;     // 괴물로 변한 상태인지
    private bool isWalk = false;

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

        if (!TryGetComponent<NavMeshAgent>(out na))
        {
            Debug.Log("EnemyMove - Failed to Load NavMeshAgent");
        }
        na.speed = moveSpeed;
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.GameModeChange>(ModeChange);
        EventBus.Instance.Subscribe<GameEvents.UpdateEnemy>(OnChange);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GameModeChange>(ModeChange);
        EventBus.Instance.Unsubscribe<GameEvents.UpdateEnemy>(OnChange);
    }

    private void OnChange(GameEvents.UpdateEnemy evt)
    {
        curState = ec.CurEnemyState;
    }

    private void FixedUpdate()
    {
        if (moveable)
        {
            AIChasePlayer();
        }
    }

    private void AIChasePlayer()
    {
        if (curState == EnemyState.Chase)
        {
            na.SetDestination(player.position);

            // 애니메이션
            Vector3 moveDir = na.velocity.normalized;
            float speed = na.velocity.magnitude;

            // speed값에 따른 iswalk
            if (speed > 0.1f)
            {
                isWalk = true;
                na.isStopped = false;
            }
            else {
                isWalk = false;
                na.isStopped = true;
                na.velocity = Vector3.zero;
                moveDir = Vector3.zero;
            }

            if (Vector3.Distance(transform.position, player.position) <= na.stoppingDistance + 0.3f)
            {
                speed = 0f;
                moveDir = Vector3.zero;
                na.isStopped = true;
                na.velocity = Vector3.zero;
                isWalk = false;
                //  Idle
            }
            else
            {
                na.isStopped = false;
                isWalk = true;
            }
            // 애니메이션 파라미터 전달
            anim.SetBool("isMonster", isMonster);
            anim.SetBool("isWalk", isWalk);
            anim.SetFloat("xDir", moveDir.x);
            anim.SetFloat("yDir", moveDir.y);
        }
    }



    public void ResumeGame()
    {
        moveable = true;
    }

    public void StopGame()
    {
        moveable = false;
    }

    public void ModeChange(GameEvents.GameModeChange evt)
    {
        if (evt.mode == GameMode.EventMode || evt.mode == GameMode.DialogMode || evt.mode == GameMode.GameOverMode)
        {
            StopGame();
        }
        else
        {
            ResumeGame();
        }
    }
}
