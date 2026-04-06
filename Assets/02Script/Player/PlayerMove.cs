using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEvents;


/// <summary>
/// 3인칭용 PlayerMove
/// </summary>
public class PlayerMove : MonoBehaviour, IMoveObject
{
    [Header("Movement Parameter")]
    [SerializeField] private float moveSpeed = 5f; // 이동속도
    [SerializeField] private float runSpeed = 10f; // 달리기 속도

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 1.5f;
    [SerializeField] private float minAngle = -60f;
    [SerializeField] private float maxAngle = 60f;

    [Header("Animator")]
    [SerializeField] private Animator anim;     // 플레이어 애니메이터
    [SerializeField] private Animator handAnim;     // 1인칭 손 애니메이터

    [Header("Ref")]
    [SerializeField] private Transform Player;
    [SerializeField] private Transform sightCam;
    [SerializeField] private LayerMask ground;

    private PlayerController pc;
    private IInputHandler inputHandler;
    private CharacterController cc;
    private bool moveable = true;
    private bool isThree = true;  // 3인칭 시점인지

    // 이동 변수
    public Vector3 moveDir = Vector3.zero;
    private Vector3 lookingDir; // 플레이어가 보는 방향
    private Vector2 input;
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded = true;
    private float gravity = -9.8f;
    private Vector3 verticalDir = Vector3.zero;   // 중력 벡터
    private Vector3 preDir = Vector3.back; // 전 프레임 이동벡터 (기본은 정면)
    private Vector3 moveInput;
    private float gravityY = 0f;

    // 스태미너 따른 플래그
    public bool canRunning = true;     // 뛸 수 있는지
    public bool forceRunning = false;  // 스태미너 고갈로 인한 강제 걷기전환


    // 카메라(1인칭)
    private float cameraVertical = 0f;

    // 누른 키 상태 저장용 변수
    private int horizontalPriority = 0; // -1:왼  1:오
    private int verticalPriority = 0;   // -1:아래  1:위


    // 플레이어가 바라보는 방향
    public Vector3 LookingDir => lookingDir;
    // 3인칭
    public Vector3 LookingDirOnThree => preDir;     // 직전 프레임이 향하는 방향

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
        //if (!TryGetComponent<Animator>(out anim)) Debug.Log("PlayerMove - Failed to Load PlayerAnimator");
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("PlayerMove - Failed to Load PlayerController");

        // 시야캠 초기화
        if (!isThree) { 
            cameraVertical = sightCam.localEulerAngles.x;
            if (cameraVertical > 180f)
                cameraVertical -= 360f;
        }
    }

    private void Update()
    {
        if (moveable && isThree && !GameManager.IsPaused)
        {
            Movement();
            ApplyGravity();
        }
        else if (moveable && !isThree && !GameManager.IsPaused)
        {
            HandleMouse();
            HandleMovement();
            ApplyGravity();
        }
    }
    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.GameModeChange>(ModeChange);
        EventBus.Instance.Subscribe<GameEvents.AspectChange>(OnAspectChange);

        // 스태미너 관련 이벤트 구독
        EventBus.Instance.Subscribe<GameEvents.OnStaminaChanged>(OnStaminaChange);
        EventBus.Instance.Subscribe<GameEvents.OnStaminaDepleted>(OnStaminaDepleted);
        EventBus.Instance.Subscribe<GameEvents.OnStaminaRecovered>(OnStaminaRecovered);

        //EventBus.Instance.Subscribe<GameEvents.SwitchScene>(OnSwitchScene);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GameModeChange>(ModeChange);
        EventBus.Instance.Unsubscribe<GameEvents.AspectChange>(OnAspectChange);

        EventBus.Instance.Unsubscribe<GameEvents.OnStaminaChanged>(OnStaminaChange);
        EventBus.Instance.Unsubscribe<GameEvents.OnStaminaDepleted>(OnStaminaDepleted);
        EventBus.Instance.Unsubscribe<GameEvents.OnStaminaRecovered>(OnStaminaRecovered);
        
        //EventBus.Instance.Subscribe<GameEvents.SwitchScene>(OnSwitchScene);
    }


    private void OnAspectChange(GameEvents.AspectChange evt) {
        if (evt.mode == AspectMode.ThirdpersonMode)
        {
            isThree = true;
            pc.CurAspect = AspectMode.ThirdpersonMode;
        }

        else { 
            isThree = false;
            pc.CurAspect = AspectMode.OnepersonMode;
        }
    }



    /// <summary>
    /// 3인칭 용
    /// </summary>
    private void Movement()
    {
        InputPriority(); // 키 입력 우선순위 갱신

        Vector2 inputVec = new Vector2(horizontalPriority, verticalPriority);
        moveInput = new Vector3(inputVec.x, 0f, inputVec.y);


        // 스태미너 고갈로 달리기 불가능한 경우 달릴 수 없음
        bool isRunning = inputHandler.Run() && inputVec.sqrMagnitude > 0.1f && canRunning && !forceRunning;
        bool isWalking = inputVec.sqrMagnitude > 0.1f && !isRunning;


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
            if (isRunning && inputVec.sqrMagnitude > 0.01f && canRunning) // Run
            {
                pc.CurState = PlayerState.Run;
                cc.Move(moveDir * (runSpeed * Time.deltaTime));
            }
            else if (!isRunning)  // Walk
            {
                pc.CurState = PlayerState.Walk;
                cc.Move(moveDir * (moveSpeed * Time.deltaTime));
            }
        }

        Vector3 localInput = transform.InverseTransformDirection(preDir); // 로컬

        // (Animation) Blend Tree 값 전달
        anim.SetBool("moveable", moveable);
        if (moveable)
        {
            // Player Animator
            anim.SetFloat("inputX", localInput.x);
            anim.SetFloat("inputY", localInput.z);
            anim.SetBool("isWalk", isWalking);
            anim.SetBool("isRunning", isRunning);
        }
    }

    /// <summary>
    /// 1인칭 용
    /// </summary>
    private void HandleMouse()
    {
        if(GameManager.IsPaused) return;

        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Y값은 카메라에 적용
        cameraVertical -= inputY;
        cameraVertical = Mathf.Clamp(cameraVertical, minAngle, maxAngle);
        sightCam.localEulerAngles = Vector3.right * cameraVertical;

        // X값은 플레이어 자체 회전
        Player.Rotate(Vector3.up * inputX);
    }

    private void HandleMovement()
    {
        Vector2 inputDir = inputHandler.GetMovement().normalized;
        Vector3 forward = sightCam.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = sightCam.right;
        right.y = 0f;
        right.Normalize();

        Vector3 moveDir = forward * inputDir.y + right * inputDir.x;

        bool isRunning = inputHandler.Run() && inputDir.sqrMagnitude > 0.1f && canRunning && !forceRunning;
        bool isWalking = inputDir.sqrMagnitude > 0.1f && !isRunning;

        float speed = isRunning ? runSpeed : moveSpeed;

        cc.Move(moveDir * speed * Time.deltaTime);

        // PlayerState분기
        if (inputDir.sqrMagnitude < 0.01f && !isRunning)
        {
            pc.CurState = PlayerState.Idle;
        }
        else {
            if (isRunning) pc.CurState = PlayerState.Run;
            else if (isWalking) pc.CurState = PlayerState.Walk;
        }


        // (Animation) Blend Tree 값 전달
        anim.SetBool("moveable", moveable);
        if (moveable)
        {
            // Player Animator
            anim.SetFloat("inputX", inputDir.x);
            anim.SetFloat("inputY", inputDir.y);
            anim.SetBool("isWalk", isWalking);
            anim.SetBool("isRunning", isRunning);

            float speedVal = 1f;
            if (isRunning) speedVal = 2f;
            else if (isWalking) speedVal = 1f;
            
            // Hand Animator
            if (handAnim != null)
            {
                handAnim.SetBool("isMove", inputDir.sqrMagnitude > 0.01f);
                handAnim.SetFloat("Speed", speedVal);
            }
        }
    }




    // 중력구현 
    private void ApplyGravity()
    {
        // 레이캐스트를 땅방향으로 쏴서 하기
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        isGrounded = Physics.Raycast(ray, 1f, ground);

        // 땅에 닿았는지 체크
        isGrounded = Physics.Raycast(ray, 1f, ground);

        if (isGrounded && gravityY < 0f)
        {
            gravityY = -2f;
        }
        else {
            gravityY += gravity * Time.deltaTime;
        }

        // 수직 방향 속도로 이동 처리
        Vector3 verticalMove = Vector3.up * gravityY * Time.deltaTime;
        cc.Move(verticalMove);
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

    // 특정(움직이지 못하는)모드 시 이전이동벡터를 이용한 Idle 애니메이션 강제
    public void ForceAnimate() {
        Vector3 dir = preDir;
        Vector3 localDir = transform.InverseTransformDirection(dir);

        anim.SetFloat("lookX", localDir.x);
        anim.SetFloat("lookY", localDir.z);

        anim.SetBool("isWalk", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("moveable", false);
    }



    public void StopGame()
    {
        moveable = false;
        ForceAnimate();
    }

    public void ResumeGame()
    {
        moveable = true;
    }

    // 스태미너가 0이 되었을 때
    private void OnStaminaDepleted(GameEvents.OnStaminaDepleted evt)
    {
        // 스태미너가 다 고갈됐을 때
        canRunning = false;
        forceRunning = true;        // 강제로 걷기 전환
        
        anim.SetBool("isRunning", false);       // 애니메이터
        pc.CurState = PlayerState.Walk;
        //Debug.Log("스태미나 고갈 !!! 달리기 제한");
    }

    // 스태미너 충전 시 최소값 만족하면 다시 달릴 수 있음
    private void OnStaminaChange(GameEvents.OnStaminaChanged evt) {
        // todo : 최소치 변경하고싶으면 여기서!!!!!!!!!!!!
        if (!canRunning && evt.curStamina >= 33f)
        {
            canRunning = true;
            forceRunning = false;
            //Debug.Log("달리기 다시 가능!");
        }
        // 스태미나가 최소값 아래일때 달릴 수 없음 (달리기 시작이 불가능)
        else if (evt.curStamina < 33f && !IsPlayerRunning())
        {
            canRunning = false;
            forceRunning = true;
            //Debug.Log("스태미너 부족으로 달릴 수 없음!");
        }
    }

    // 스태미너가 모두 회복되었을 때 호출 (필요하진 않으나, 혹시 다채워졌을 때 필요하다면?)
    private void OnStaminaRecovered(GameEvents.OnStaminaRecovered evt)
    {
        canRunning = true;
        forceRunning = false;
        //Debug.Log("스태미너 완전 회복 - 달리기 가능!");
    }



    public void ModeChange(GameEvents.GameModeChange evt) {
        if (evt.mode == GameMode.EventMode || evt.mode == GameMode.DialogMode || evt.mode == GameMode.GameOverMode ||
            evt.mode == GameMode.PauseMode || evt.mode == GameMode.UIPuzzleMode)
        {
            StopGame();
        }
        else
        {
            ResumeGame();
        }

        // 특정모드 (EventInInspectMode)에서는 달리기 금지 (걷기만 가능)
        if (evt.mode == GameMode.EventInInspectMode)
        {
            canRunning = false;
            forceRunning = true;
        }
        else if (evt.mode != GameMode.EventInInspectMode) // !forceRunning < 조건 삭제
        {
            canRunning = true;
            forceRunning = false;
        }
    }


    // 외부에서 호출할 Player BackStep
    public void PushBack() {
        Vector3 backDir = -preDir.normalized;
        cc.Move(backDir);

        Debug.Log("뒤로밀림");
    }

    // 외부에서 호출할 Player 방향 강제수정
    public void SetPlayerDir(Vector2 dir) {
        preDir = dir.normalized;

        // 애니메이터
        Vector3 animDir = transform.InverseTransformDirection(dir);

        if (anim != null) {
            // 이동 블렌드트리
            anim.SetFloat("inputX", animDir.x);
            anim.SetFloat("inputY", animDir.z);

            anim.SetFloat("lookX", animDir.x);
            anim.SetFloat("lookY", animDir.z);
        }
    }


    // 현재 달리는 중인지?
    private bool IsPlayerRunning() {
        return pc.CurState == PlayerState.Run;
    }
}
