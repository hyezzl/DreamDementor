using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayerMove : MonoBehaviour, IMoveObject
{
    [Header("Movement Parameter")]
    [SerializeField] private float moveSpeed = 5f; // 이동속도
    [SerializeField] private float runSpeed = 10f; // 달리기 속도
    [SerializeField] private LayerMask ground;

    [Header("Animator")]
    [SerializeField] private Animator anim;

    private PlayerController pc;
    private IInputHandler inputHandler;
    private CharacterController cc;
    private bool moveable = true;

    // 이동 변수
    public Vector3 moveDir = Vector3.zero;
    private Vector3 lookingDir; // 플레이어가 보는 방향
    private Vector2 input;
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded = true;
    private float gravity = -9.8f;
    private Vector3 verticalDir = Vector3.zero;   // 중력 벡터
    private Vector3 preDir = Vector3.back; // 전 프레임 이동벡터 (기본은 정면)

    // 누른 키 상태 저장용 변수
    private int horizontalPriority = 0; // -1:왼  1:오
    private int verticalPriority = 0;   // -1:아래  1:위


    public Vector3 LookingDir => lookingDir;

    // 플레이어 애니메이션 연출위함
    public Vector3 PreDir {
        get => preDir;
        set => preDir = value;
    }

    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;


    private void Awake()
    {
        if (!TryGetComponent<CharacterController>(out cc))
        {
            Debug.Log("PlayerMove - Failed to Load CharacterController");
        }
        if (!TryGetComponent<PlayerController>(out pc)) {
            Debug.Log("PlayerMove - Failed to Load PlayerController");
        }
    }

    private void Update()
    {
        //ApplyGravity();
        if (moveable) {
            Movement();
        }

    }
    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.GameModeChange>(ModeChange);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GameModeChange>(ModeChange);
    }



    private void Movement()
    {
        InputPriority(); // 키 입력 우선순위 갱신
        Vector2 inputVec = new Vector2(horizontalPriority, verticalPriority);
        Vector3 moveInput = new Vector3(inputVec.x, 0f, inputVec.y);

        // 입력값 받기
        //input = inputHandler.GetMovement();
        bool isRunning = inputHandler.Run();
        bool isWalking = inputVec.sqrMagnitude > 0.1f && !isRunning;
        //Vector3 moveInput = new Vector3(input.x, 0f, input.y);


        if (pc.CurAspect == AspectMode.ThirdpersonMode) // 3인칭
        {
            moveDir = moveInput.normalized + verticalDir;

        }
        else // 1인칭 
        {
            Vector3 horizonDir = transform.TransformDirection(moveInput);  // 로컬방향 변환
            horizonDir.y = 0f;
            moveDir = horizonDir + verticalDir;
        }

        // PlayerState 분기 (유저 입력에 따른)
        if (inputVec.sqrMagnitude < 0.001f && !isRunning) //Idle
        {
            pc.CurState = PlayerState.Idle;
        }
        else  // 사용자 move 움직임 있을 때
        {
            if (moveInput.sqrMagnitude > 0.001f) // 외부에서 preDir 설정 안 했을 경우
            { 
                preDir = moveInput.normalized;   // 마지막 이동방향 캐싱
            }
            if (!isRunning) // Walk
            {
                pc.CurState = PlayerState.Walk;
                cc.Move(moveDir * (moveSpeed * Time.deltaTime));
            }
            else if (isRunning && inputVec.sqrMagnitude > 0.01f)  // Run
            {
                pc.CurState = PlayerState.Run;
                cc.Move(moveDir * (runSpeed * Time.deltaTime));
            }
        }

        Vector3 localInput = transform.InverseTransformDirection(preDir); // 로컬

        // (Animation) Blend Tree 값 전달
        anim.SetBool("moveable", moveable);
        if (moveable)
        {
            anim.SetFloat("inputX", localInput.x);
            anim.SetFloat("inputY", localInput.z);
            anim.SetBool("isWalk", isWalking);
        }

    }

    // 중력구현 
    private void ApplyGravity()
    {
        // 레이캐스트를 땅방향으로 쏴서 하기
        ////////////////////// 1.
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        isGrounded = Physics.Raycast(ray, 1f, ground);

        //bool isGrounded = cc.isGrounded;  // 1. cc가 땅레이어와 부딪힌걸 인식못함? 

        //////////////////////2
        //Vector3 sphere = transform.position + Vector3.down * (cc.height / 2);
        //isGrounded = Physics.CheckSphere(sphere, cc.radius, ground);

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -1f;
            Debug.Log("tq");
        }
        velocity.y += gravity * Time.deltaTime;
        verticalDir = Vector3.up * velocity.y;

        // 땅에 붙어있는걸 인식못해서 중력이 계속 누적되는 문제
    }


    // 키 눌림 시간 기록용 변수
    private float lastLeftTime = -1f;
    private float lastRightTime = -1f;
    private float lastUpTime = -1f;
    private float lastDownTime = -1f;
    private void InputPriority()
    {
        float now = Time.time;

        // 누른 순간 기록
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) lastLeftTime = now;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) lastRightTime = now;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) lastUpTime = now;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) lastDownTime = now;

        bool leftHeld = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool rightHeld = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool upHeld = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool downHeld = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        // 수평 방향 우선순위
        if (leftHeld && rightHeld)
            horizontalPriority = lastLeftTime > lastRightTime ? -1 : 1;
        else if (leftHeld)
            horizontalPriority = -1;
        else if (rightHeld)
            horizontalPriority = 1;
        else
            horizontalPriority = 0;

        // 수직 방향 우선순위
        if (upHeld && downHeld)
            verticalPriority = lastUpTime > lastDownTime ? 1 : -1;
        else if (upHeld)
            verticalPriority = 1;
        else if (downHeld)
            verticalPriority = -1;
        else
            verticalPriority = 0;
    }



    public void StopGame()
    {
        moveable = false;
    }

    public void ResumeGame()
    {
        moveable = true;
    }

    public void ModeChange(GameEvents.GameModeChange evt) {
        if (evt.mode == GameMode.EventMode || evt.mode == GameMode.DialogMode || evt.mode == GameMode.GameOverMode)
        {
            StopGame();
        }
        else {
            ResumeGame();
        }
    }
}
