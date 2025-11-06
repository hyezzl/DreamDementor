using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// 3인칭 내 괴물
/// </summary>

public class EnemyMove : MonoBehaviour, IMoveObject
{
    [SerializeField] private Transform player;
    public EnemyState curState;
    public bool moveable;

    [Header("Enemy Movement Setting")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rig;
    private EnemyController ec;
    private PlayerController pc;
    private NavMeshAgent na;
    private Animator anim;
    private Billboard billboard;    // 빌보드는 항상 켜있어야 하지않을까?

    private bool isWalk = false;
    private Vector3 preMoveDir = Vector3.zero;      // 이전프레임 이동각도


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

        if (!TryGetComponent<Animator>(out anim)) Debug.Log("EnemyMove - Failed to Load Animator");

        if (!TryGetComponent<Billboard>(out billboard)) Debug.Log("EnemyMove - Failed to Load Billboard");

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


            
            /////////// 관성 제어 ///////////
            ///
            // 방향변화 감지
            float deltaDir = Vector3.Angle(preMoveDir, (player.position - transform.position).normalized);

            // 이전 프레임과 현재 프레임 방향차이가 특정각도 이상하면 멈춤 (관성 제거)
            if (deltaDir > 45f)
            {
                na.isStopped = true;
            }
            preMoveDir = (player.position - transform.position).normalized;

            // 플레이어 이동에 따른 벡터 기준 변경
            Vector3 playerForward = player.forward;
            playerForward.y = 0f;
            playerForward.Normalize();

            // 플레이어의 오른쪽 벡터
            Vector3 playerRight = Vector3.Cross(Vector3.up, playerForward);

            // 상대적 방향값 구하기
            float relX = Vector3.Dot(moveDir, playerRight);
            float relY = Vector3.Dot(moveDir, playerForward);

            // 각도
            float angle = Vector3.SignedAngle(playerForward, moveDir, Vector3.up);

            // speed값에 따른 iswalk
            if (speed > 0.1f)
            {
                isWalk = true;
                na.isStopped = false;

                // 애니메이터 파라미터 업데이트
                anim.SetFloat("dirX", relX);
                anim.SetFloat("dirY", relY);
            }
            else
            {
                isWalk = false;
                na.isStopped = true;
                na.velocity = Vector3.zero;
                moveDir = Vector3.zero;

                // 애니메이터 파라미터 업데이트
                anim.SetFloat("dirX", 0);
                anim.SetFloat("dirY", 0);
            }

            // 목적지 도달 시 멈춤
            if (Vector3.Distance(transform.position, player.position) <= na.stoppingDistance + 0.3f)
            {
                speed = 0f;
                moveDir = Vector3.zero;
                na.isStopped = true;
                na.velocity = Vector3.zero;
                isWalk = false;

                anim.SetFloat("dirX", 0);
                anim.SetFloat("dirY", 0);
            }
            else
            {
                na.isStopped = false;
                isWalk = true;
            }

            // 애니메이터 파라미터 전달
            anim.SetBool("isWalk", isWalk);
        }
    }



    public void ResumeGame()
    {
        moveable = true;
    }

    public void StopGame()
    {
        moveable = false;
        if (na != null)
        {
            na.isStopped = true;
            na.velocity = Vector3.zero;
        }
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
