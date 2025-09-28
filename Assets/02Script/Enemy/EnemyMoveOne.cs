using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveOne : MonoBehaviour, IMoveObject
{
    [SerializeField] private Transform player;
    [SerializeField] private Animator anim;
    public EnemyState curState;
    public bool moveable = true;

    [Header("Enemy Movement Setting")]
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody rig;
    private EnemyController ec;
    private PlayerController pc;
    private NavMeshAgent na;

    // 상태값
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

            // 플레이어 이동에 따른 벡터 기준 변경
            Vector3 playerForward = player.forward;
            playerForward.y = 0f;
            playerForward.Normalize();

            // 플레이어의 오른쪽 벡터
            Vector3 playerRight = Vector3.Cross(Vector3.up, playerForward);

            // 적 이동 방향 플레이어 기준 상대좌표 변환
            float relX = Vector3.Dot(moveDir, playerRight);
            float relY = Vector3.Dot(moveDir, playerForward);

            // 각도
            float angle = Vector3.SignedAngle(playerForward, moveDir, Vector3.up);

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

            // 애니메이션 파라미터 전달 (조건부)
            //if (Mathf.Abs(angle) > 60f)
            //{
            //    // 완전 옆 방향일때만 Left/Right 넘김 (좌우)
            //    anim.SetFloat("xDir", relX);
            //    anim.SetFloat("yDir", 0);
            //}
            //else {
            //    anim.SetFloat("xDir", 0);
            //    anim.SetFloat("yDir", relY);
            //}
            //anim.SetBool("isWalk", isWalk);
            //anim.SetFloat("xDir", relX);
            //anim.SetFloat("yDir", relY);
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
