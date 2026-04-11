using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
using static GameEvents;
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

    [Header("Detection Setting")]
    [SerializeField] private float lostLimit = 10f;  // 어그로 풀리는 시간
    [SerializeField] float lostTimer = 0f;
    private bool isPlayerInVision = false;           // 플레이어가 괴물 시야범위내에 있는지

    [Header("Patrol Setting")]
    public Transform[] patrolPoints;
    private int curPatrolPointIdx = 0;

    [Header("Vision Ref")]
    [SerializeField] private Transform vision;

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
        EventBus.Instance.Subscribe<GameEvents.UseCabinet>(OnPlayerEnterCabinet);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GameModeChange>(ModeChange);
        EventBus.Instance.Unsubscribe<GameEvents.UpdateEnemy>(OnChange);
        EventBus.Instance.Unsubscribe<GameEvents.UseCabinet>(OnPlayerEnterCabinet);
    }

    private void OnChange(GameEvents.UpdateEnemy evt)
    {
        curState = ec.CurEnemyState;
    }

    private void FixedUpdate()
    {
        if (!moveable) return;

        switch (curState) 
        {
            case EnemyState.Patrol:
                AIPatrol();
                break;

            case EnemyState.Chase:
                AIChasePlayer();
                break;

            case EnemyState.LostTarget:
                AILostTarget();
                break;
        }
    }



    #region !!!!!!!!!!  주요 상태별 로직  !!!!!!!!!!!

    // Chase
    private void AIChasePlayer()
    {
        na.isStopped = false;
        na.SetDestination(player.position);

        // 관성 제어 (급커브 시 멈춤)
        ControlInertia();

        // 애니메이션 및 플레이어 상대 좌표(relX, relY) 계산 포함
        UpdateAnimationAndDirection(na.velocity);

        // 플레이어와 너무 가까우면 정지
        if (Vector3.Distance(transform.position, player.position) <= na.stoppingDistance + 0.3f)
        {
            StopMovementAction();
        }

        // 시야 밖으로 나갈 경우 카운트 시작
        if (!isPlayerInVision) {
            lostTimer = 0f; // 카운트 초기화
            EventBus.Instance.Publish(new GameEvents.EnemyStateChange(EnemyState.LostTarget));
        }
    }

    // 10초 카운트 로직
    private void AILostTarget()
    {
        // 타겟을 잃었어도 플레이어 위치로 계속 이동
        na.SetDestination(player.position);
        UpdateAnimationAndDirection(na.velocity);

        // 시야 안에 들어오면 리셋
        if (isPlayerInVision)
        {
            // 타이머 리셋
            lostTimer = 0f;
            EventBus.Instance.Publish(new GameEvents.EnemyStateChange(EnemyState.Chase));
            return;
        }

        lostTimer += Time.fixedDeltaTime;

        // 특정 시간에 도달하면 패트롤
        if (lostTimer >= lostLimit) {
            Debug.Log("괴물이 다시 순찰모드로 전환");
            EventBus.Instance.Publish(new GameEvents.EnemyStateChange(EnemyState.Patrol));
        }
    }


    // 패트롤(순찰)모드
    private void AIPatrol()
    {
        if (patrolPoints == null || patrolPoints.Length < 2) return;

        na.isStopped = false;
        na.SetDestination(patrolPoints[curPatrolPointIdx].position);
        UpdateAnimationAndDirection(na.velocity);

        ///
        if (!na.pathPending && na.remainingDistance < 0.5f) { 
            curPatrolPointIdx = (curPatrolPointIdx + 1) % patrolPoints.Length;
        }
    }

    // 캐비닛 생사 판정
    private void OnPlayerEnterCabinet(GameEvents.UseCabinet evt) 
    {
        if (curState == EnemyState.Patrol) return;

        if (isPlayerInVision && (curState == EnemyState.Chase || curState == EnemyState.LostTarget))
        {
            Debug.Log("캐비닛 안에 있는 플레이어를 귀신이 찾아냄 사망");
            // 플레이어 사망
        }
    }

    #endregion

    #region --------- 보조

    public void SetPlayerInVision(bool inVision)
    {
        isPlayerInVision = inVision;
    }

    // 관성 제어 로직
    private void ControlInertia()
    {
        Vector3 currentTargetDir = (player.position - transform.position).normalized;
        float deltaDir = Vector3.Angle(preMoveDir, currentTargetDir);

        if (deltaDir > 45f)
        {
            na.isStopped = true;
        }
        preMoveDir = currentTargetDir;
    }

    // 애니메이션 및 상대적 방향(relX, relY) 업데이트
    private void UpdateAnimationAndDirection(Vector3 velocity)
    {
        float speed = velocity.magnitude;
        Vector3 moveDir = velocity.normalized;

        if (speed > 0.1f)
        {
            isWalk = true;
            na.isStopped = false;

            // 애니메이션 방향 계산
            Vector3 playerForward = player.forward;
            playerForward.y = 0f;
            playerForward.Normalize();
            Vector3 playerRight = Vector3.Cross(Vector3.up, playerForward);

            // 상대적 방향값 (relX, relY)
            float relX = Vector3.Dot(moveDir, playerRight);
            float relY = Vector3.Dot(moveDir, playerForward);

            anim.SetFloat("dirX", relX);
            anim.SetFloat("dirY", relY);

            // 애니메이션 파라미터 기준으로 시야 회전
            float absX = Mathf.Abs(relX);
            float absY = Mathf.Abs(relY);

            // 좌우 우선
            if (absY > absX)
            {
                if (relY > 0) // 앞 (Player가 보는 정면 방향으로 이동 중)
                    vision.localRotation = Quaternion.Euler(0, 90, 0);
                else // 뒤
                    vision.localRotation = Quaternion.Euler(0, 270, 0);
            }
            else // 좌/우 방향이 지배적일 때
            {
                if (relX > 0) // 오른쪽
                    vision.localRotation = Quaternion.Euler(0, 180, 0);
                else // 왼쪽
                    vision.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            isWalk = false;
            anim.SetFloat("dirX", 0);
            anim.SetFloat("dirY", 0);
        }

        anim.SetBool("isWalk", isWalk);
    }

    // 완전히 멈출 때 사용
    private void StopMovementAction()
    {
        isWalk = false;
        na.isStopped = true;
        na.velocity = Vector3.zero;
        anim.SetFloat("dirX", 0);
        anim.SetFloat("dirY", 0);
        anim.SetBool("isWalk", false);
    }

    //private void InitPatrolPoints()
    //{
    //    GameObject group = GameObject.Find("PatrolGroup");
    //    if (group != null)
    //    {
    //        patrolPoints = group.GetComponentsInChildren<Transform>().Where(t => t != group.transform).ToArray();
    //    }
    //}

    #endregion



    #region -- 게임 모드 및 상태 제어 --

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
    #endregion

}
