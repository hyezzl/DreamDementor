using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private LayerMask itemLayer;  // 아이템 레이어 참조

    private PlayerController pc;
    private PlayerMove pm;
    private IInputHandler inputHandler;
    private IActionItem curItem; // 바라보고있는 아이템
    private float maxDistance = 100f;  // 시야 닿는 최대거리

    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        if (pc == null) Debug.Log("Interact - Failed to Load PlayerController");

        if (!TryGetComponent<PlayerMove>(out pm)) {
            Debug.Log("Interact - Failed to Load PlayerMove");
        }
    }


    // 선택지 중 : DoSelect
    // 탐색 중 : DoInteract

    private void Update()
    {
        OnInteract();
    }


    private void OnInteract() {
        if (pc.CurMode == GameMode.InspectMode) { 
            if (inputHandler.DoInteract() && SearchForward() != null) {
                SearchForward().Interact(); // PickableItem.Interact();
            }
        }
    }

    // raycast로 상호작용 할 아이템 탐색
    private IActionItem SearchForward() {
        // ItemLayer만을 검출
        RaycastHit2D hit = Physics2D.Raycast(transform.position, pm.LookingDir, maxDistance, itemLayer);
        if (hit.collider != null) {
            Debug.Log($"raycast 결과 : {hit.collider.name}");
            return hit.collider.GetComponent<IActionItem>();
        }
        return null;
    }
}


