using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static GameEvents;

public class Horror02_Enemy : MonoBehaviour, IMoveObject
{
    [SerializeField] private Transform player;
    public EnemyState curState;
    public bool moveable;

    [Header("Enemy Movement Setting")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Patrol Setting")]
    public Transform patrolPoint;

    [Header("Vision Ref")]
    [SerializeField] private Transform vision;

    private bool isPlayerInVision = false;
    private bool isArrived = false;     // 목적지에 도착했는지

    private Rigidbody rig;
    private EnemyController ec;
    private PlayerController pc;
    private NavMeshAgent na;
    private Animator anim;

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

        na.updateRotation = false;      // 자동회전값 끄기
        na.speed = moveSpeed;
    }


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.GameModeChange>(ModeChange);
        EventBus.Instance.Subscribe<GameEvents.UpdateEnemy>(OnChange);
        EventBus.Instance.Subscribe<GameEvents.UseCabinet>(OnPlayerEnterCabinet);

        curState = EnemyState.Patrol;   // 초기값은 patrol
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

        if (curState == EnemyState.Patrol)
        {
            moveable = true;
            if (na != null)
            {
                na.isStopped = false;
                na.SetDestination(patrolPoint.position); // 목적지 강제 재설정
            }
        }
    }


    private void FixedUpdate()
    {
        if (!moveable) return;

        if (curState == EnemyState.Patrol)
        {
            GoToPoint();
        }
        
    }

    private void GoToPoint()
    {
        na.isStopped = false;
        // 정해진 좌표로 이동
        na.destination = patrolPoint.position;

        // 관성 제어 (급커브 시 멈춤)
        ControlInertia();

        // 애니메이션 및 플레이어 상대 좌표(relX, relY) 계산 포함
        UpdateAnimationAndDirection(na.velocity);

        // 도착지 판정 로직
        if (!na.pathPending)
        {
            // remainingDistance: 남은 거리
            if (na.remainingDistance <= na.stoppingDistance && !isArrived)
            {
                // 경로가 완전히 끝났거나 속도가 거의 없을 때
                if (!na.hasPath || na.velocity.sqrMagnitude == 0f)
                {
                    isArrived = true;
                    OnArrivalAtPoint();
                    Debug.Log("도착했음");
                }
            }
        }
    }

    // 캐비닛 생사 판정
    private void OnPlayerEnterCabinet(GameEvents.UseCabinet evt)
    {
        if (curState == EnemyState.Patrol)
        {
            Debug.Log("2층 로직 클리어!");

            // 적이 패트롤포인트 도착할때까지 입력 막음
            PlayerController.Instance.CurMode = GameMode.EventMode;
            EventBus.Instance.Publish(new GameEvents.GameModeChange(GameMode.EventMode));
        }

        if (isPlayerInVision && (curState == EnemyState.Chase || curState == EnemyState.LostTarget))
        {
            // 2층에서 적에게 들킴
            Debug.Log("2층 로직 클리어못함 죽어라");
            // 플레이어 사망
        }
    }

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

    // 도착지점 도착했을때
    private void OnArrivalAtPoint()
    {
        if (!moveable) return;

        StopGame(); // 정지

        // 이벤트 모드

        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish(new GameEvents.GameModeChange(GameMode.EventMode));

        EventBus.Instance.Publish(new GameEvents.PlayEvent("E073"));

        // 본인 오브젝트 비활성화
        StartCoroutine(SafeDestroy());
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
        if (evt.mode == GameMode.DialogMode || evt.mode == GameMode.GameOverMode ||
            evt.mode == GameMode.PauseMode || evt.mode == GameMode.UIPuzzleMode)
        {
            StopGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private IEnumerator SafeDestroy()
    { 
        yield return null;

        this.gameObject.SetActive(false);
    }
}
