using UnityEngine;

public class PlayerMove : MonoBehaviour, IMoveObject
{
    [Header("Movement Parameter")]
    [SerializeField] private float moveSpeed = 5f; // 이동속도
    [SerializeField] private float runSpeed = 10f; // 달리기 속도

    [Header("PlayerSprite Refs")]
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;
    [SerializeField] private Transform player;


    private IInputHandler inputHandler;
    private CharacterController cc;

    private void Awake()
    {
        if (!TryGetComponent<IInputHandler>(out inputHandler))
        {
            Debug.Log("PlayerMove - Failed to Load IInputHandler");
        }
        if (!TryGetComponent<CharacterController>(out cc))
        {
            Debug.Log("PlayerMove - Failed to Load CharacterController");
        }
    }

    private void Update()
    {
        TempMove();
    }

    private void TempMove() {
        Vector2 dir = inputHandler.GetMovement();

        Vector3 moveInput = new Vector3(dir.x, 0f, dir.y).normalized;
        // 로컬 방향으로 변환
        Vector3 movedir = player.transform.TransformDirection(moveInput);

        cc.Move(movedir * (moveSpeed * Time.deltaTime));
    }

    public void StopGame()
    {
    }

    public void ResumeGame()
    {
    }
}
