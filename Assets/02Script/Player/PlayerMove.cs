using UnityEngine;

public class PlayerMove : MonoBehaviour, IMoveObject
{
    [Header("Movement Parameter")]
    [SerializeField] private float moveSpeed = 5f; // 이동속도
    [SerializeField] private float runSpeed = 10f; // 달리기 속도
    [SerializeField] private LayerMask ground;

    private PlayerController pc;
    private IInputHandler inputHandler;
    private CharacterController cc;
    private Animator anim;
    private bool moveable = true;

    // 이동 변수
    public Vector3 moveDir = Vector3.zero;
    private Vector3 lookingDir; // 플레이어가 보는 방향
    private Vector2 input;
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded = true;
    private float gravity = -9.8f;
    private Vector3 verticalDir = Vector3.zero;   // 중력 벡터


    public Vector3 LookingDir => lookingDir;

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
        if (!TryGetComponent<Animator>(out anim)) {
            Debug.Log("PlayerMove - Failed to Load Animator");
        }
    }

    private void Update()
    {
        //ApplyGravity();
        if (moveable) Movement();
    }

    private void Movement() {
        // 입력값 받기
        input = inputHandler.GetMovement();
        bool isRunning = inputHandler.Run();

        if (pc.CurAspect == AspectMode.ThirdpersonMode) // 3인칭
        {
            Vector3 moveInput = new Vector3(input.x, 0f, input.y).normalized;
            moveDir = moveInput.normalized + verticalDir;  // 2. 정규화 한번만해서? 중력만 큰값으로 합쳐지는경우

        }
        else // 1인칭 
        {
            Vector3 moveInput = new Vector3(input.x, 0f, input.y).normalized;
            Vector3 horizonDir = transform.TransformDirection(moveInput);  // 로컬방향 변환
            horizonDir.y = 0f;

            //moveDir = horizonDir.normalized + verticalDir;
            moveDir = horizonDir + verticalDir;
        }

        if (input.sqrMagnitude < 0.01f && !isRunning) //Idle
        {
            pc.CurState = PlayerState.Idle;
            Debug.Log("Idle");
        }
        else if (!isRunning) // Walk
        {
            pc.CurState = PlayerState.Walk;
            cc.Move(moveDir * (moveSpeed * Time.deltaTime));
            Debug.Log("Walk");
        }
        else if(isRunning && input.sqrMagnitude > 0.01f)  // Run
        {
            pc.CurState = PlayerState.Run;
            cc.Move(moveDir * (runSpeed * Time.deltaTime));
            Debug.Log("Run");
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
        //if (cc.isGrounded) Debug.Log("땅에 붙어있음");
        //else Debug.Log("ㄴㄴ");

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

    // 애니메이션 변경
    private void PlayerAnim() {

        input = inputHandler.GetMovement();
        bool isRunning = inputHandler.Run();

        //switch (pc.CurState) {
        //    case PlayerState.Idle:
        //        anim.SetBool()
        //        break;
        //    case PlayerState.Walk:
        //        break;
        //    case PlayerState.Run:
        //        break;
        //}
        if (input.sqrMagnitude < 0.01f && !isRunning) // Idle
        {
            anim.SetBool("iswalking", false);
        }
        else if (!isRunning)  // Walk
        { 
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
