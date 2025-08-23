using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private IInputHandler inputHandler;
    private CharacterController cc;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform player;

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
}
