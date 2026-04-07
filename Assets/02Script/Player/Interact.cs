using DG.Tweening;
using System.Collections;
using UnityEngine;


/// <summary>
/// 3인칭 : 박스캐스트 사용
/// 
/// 1인칭 : 시야 콜라이더 사용
/// </summary>
public class Interact : MonoBehaviour
{
    [Header("InteractBox Setting")]
    [SerializeField] private float castDist;        // 플레이어 - 박스 캐스트 중심까지의 거리
    [SerializeField] private LayerMask itemLayer;  // 아이템 레이어 참조
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 half = new Vector3(0.5f, 1f, 1f); // 가로 1, 세로 2, 깊이 1
    [SerializeField] private PlayerSight sight;


    private PlayerController pc;
    private PlayerMove pm;
    private IInputHandler inputHandler;
    private IActionItem curItem; // 바라보고있는 아이템
    private AspectMode curAspect = AspectMode.ThirdpersonMode;

    public bool isChase = false;       // 쫓기고 있는가?


    // 박스 캐스트 변수
    Vector3 boxCenter;
    RaycastHit hitInfo;     // 레이캐스트 정보
    Quaternion rotate;      // 박스 회전값


    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;

    private void Awake()
    {
        if (!TryGetComponent<PlayerMove>(out pm)) {
            Debug.Log("Interact - Failed to Load PlayerMove");
        }
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.AspectChange>(OnModeChange);
        EventBus.Instance.Subscribe<GameEvents.EnemyStateChange>(OnChase);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.AspectChange>(OnModeChange);
        EventBus.Instance.Unsubscribe<GameEvents.EnemyStateChange>(OnChase);
    }


    // 선택지 중 : DoSelect
    // 탐색 중 : DoInteract

    private void Update()
    {
        OnInteract();

        if (Input.GetKeyDown(KeyCode.K)) {
            Temp();
        }
    }


    private void OnInteract() {
        if (isChase)
        {
            // 쫓기는 중이라면
            var target = (curAspect == AspectMode.ThirdpersonMode) ? SearchForward() : sight.curTarget;
            if (target == null) return;

            // 태그가 EscapeDoor 인 경우만 상호작용 허용
            if (target.GetObject().CompareTag("EscapeDoor"))
            {
                if (inputHandler.DoInteract() && SearchForward() != null)
                {
                    target.Interact();
                }
            }
            else return;

        }
        else {
            // 쫓기지 않을 때
            switch (curAspect)
            {
                case AspectMode.ThirdpersonMode:
                    if (PlayerController.Instance.CurMode == GameMode.InspectMode) { 
                        if (inputHandler.DoInteract() && SearchForward() != null)
                        {
                            {
                                SearchForward().Interact();
                            }
                        }
                    }
                    break;


                case AspectMode.OnepersonMode:
                    if (PlayerController.Instance.CurMode == GameMode.InspectMode)
                    {
                        if (inputHandler.DoInteract() && sight.curTarget != null)
                        {
                            sight.curTarget.Interact();
                        }
                    }
                    break;

            }
        }

    }


    // 3인칭 일 때
    // 박스캐스트를 이용한 물체 검색
    private IActionTarget SearchForward() {
        Vector3 lookDir = pm.LookingDirOnThree;    // 플레이어가 현재 바라보는 방향 (직전 프레임이 향하는 방향)
        if (lookDir == Vector3.zero) lookDir = player.transform.forward;

        rotate = Quaternion.LookRotation(lookDir);
        boxCenter = player.transform.position + lookDir.normalized * castDist + Vector3.up;  // 박스 시작
        
        RaycastHit[] hits = Physics.BoxCastAll(boxCenter,
                                    half,
                                    lookDir.normalized,
                                    rotate,
                                    castDist,
                                    itemLayer
                                    );

        foreach (var hit in hits) {
            var item = hit.collider.GetComponent<IActionTarget>();
            if (item != null) {
                //Debug.Log("아이템 감지! : " + hit.collider.gameObject.name);

                // 우선순위
                return item;
            }
        }
        return null;
    }

    // "씬뷰용" 박스캐스트 시각화
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;

        boxCenter = player.transform.position + player.transform.forward * castDist + Vector3.up;
        rotate = Quaternion.Euler(player.transform.rotation.eulerAngles.x - 10f, player.transform.rotation.y, player.transform.rotation.z);

        Matrix4x4 matrix = Matrix4x4.TRS(boxCenter, rotate, Vector3.one);
        Gizmos.matrix = matrix;

        Gizmos.DrawWireCube(Vector3.zero, half * 2);
    }

    private void OnChase(GameEvents.EnemyStateChange evt) {
        if (evt.state == EnemyState.Chase) {
            isChase = true;
        }
    }


    // ==============================================================


    // 1인칭 일때






    // 시점이 변하면 Interact 방식도 변경
    private void OnModeChange(GameEvents.AspectChange evt) {
        curAspect = evt.mode;
    }

    private void Temp() {
        // 현재 메모리에 뭐가 있는지 전부 출력 // 임시
        var list = EventHistoryManager.Instance.GetCompletedEventsList();
        Debug.Log($"[SorrowDoor] 현재 메모리 내 이벤트 개수: {list.Count}");
        foreach (var id in list) Debug.Log($"기록된 ID: {id}");
    }

}


