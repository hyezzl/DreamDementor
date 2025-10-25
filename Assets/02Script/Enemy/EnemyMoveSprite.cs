using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// 괴물 움직임 + 괴물 애니메이션
/// </summary>
public class EnemyMoveSprite : MonoBehaviour, IMoveObject
{
    [SerializeField] private Transform player;
    [SerializeField] private Animator anim;

    [Header("Enemy Movement Setting")]
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody rig;
    private EnemyController ec;
    private PlayerController pc;
    private NavMeshAgent na;
    private bool moveable;
    private EnemyState curState;
    private Vector3 preMoveDir = Vector3.zero;      // 이전프레임 이동각도

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
        
        if (!TryGetComponent<NavMeshAgent>(out na)) {
            Debug.Log("EnemyMove - Failed to Load NavMeshAgent");
        }
        na.updateRotation = false;      // 자동회전값 끄기
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
        if (moveable) { 
            AIChasePlayer();
            FlipSprite();
        }
    }


    private void AIChasePlayer() {
        if (curState == EnemyState.Chase)
        {
            na.SetDestination(player.position);

            // 애니메이션
            Vector3 moveDir = na.velocity.normalized;
            float speed = na.velocity.magnitude;

            // 방향변화 감지
            float deltaDir = Vector3.Angle(preMoveDir, (player.position - transform.position).normalized);

            // 이전 프레임과 현재 프레임 방향차이가 특정각도 이상하면 멈춤 (관성 제거)
            if (deltaDir > 45f) {
                na.velocity = Vector3.zero;
            }
            preMoveDir = (player.position - transform.position).normalized;

            // 목적지 도달 시 멈춤
            if (Vector3.Distance(transform.position, player.position) <= na.stoppingDistance + 0.3f)
            //if (moveDir.magnitude < 0.1f)
            {
                speed = 0f;
                moveDir = Vector3.zero;
                na.isStopped = true;
                na.velocity = Vector3.zero;
                isWalk = false;
                //  Idle
            }
            else { 
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

    private void FlipSprite() {
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);   // 왼쪽
        }
        else {
            transform.localScale = new Vector3(-1, 1, 1);    // 오른쪽
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

    public void ModeChange(GameEvents.GameModeChange evt)
    {
        if (evt.mode == GameMode.EventMode || evt.mode == GameMode.DialogMode || evt.mode == GameMode.GameOverMode ||
            evt.mode == GameMode.PauseMode || evt.mode == GameMode.UIPuzzleMode)
        {
            StopGame();
        }
        else
        {
            ResumeGame();
        }
    }
}
