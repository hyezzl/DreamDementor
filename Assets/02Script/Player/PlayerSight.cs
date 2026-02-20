using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSight : MonoBehaviour
{
    public List<IActionTarget> overlapItems = new();

    [Header("Tag")]
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private LayerMask deactiveLayer;
    
    private Camera mainCam;
    private CapsuleCollider col;
    private Rigidbody rig;

    public IActionTarget curTarget;

    private void Awake()
    {
        if (!TryGetComponent<CapsuleCollider>(out col)) Debug.Log("PlayerSight - Failed to Load CapsuleCollider");
        if (!TryGetComponent<Rigidbody>(out rig)) Debug.Log("PlayerSight - Failed to Load Rigidbody");
        mainCam = Camera.main;
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.GetItem>(ExceptItem);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GetItem>(ExceptItem);

        // 비활성화 시 리스트 초기화
        overlapItems.Clear();
    }


    // 매프레임 갱신
    private void Update()
    {
        if (overlapItems.Count > 0)
        {
            // 매프레임 갱신
            var closest = ClosestItem();

            // OutLine
            foreach (var item in overlapItems)
            {
                MonoBehaviour it = item as MonoBehaviour; // 안전성
                if (it != null && it.TryGetComponent<Outline>(out Outline outline))
                {
                    // 내가 현재 바라보는 아이템이면 on
                    outline.enabled = (item == closest);
                }
            }

            if (curTarget != closest)
            {
                curTarget = closest;
            }
        }
        else {
            curTarget = null;
        }
        
    }


    // 시야콜라이더에 들어왔을 때 리스트에 추가
    private void OnTriggerEnter(Collider other)
    {
        int layer = other.gameObject.layer;

        // 태그로 분리
        if (other.CompareTag("Item")) 
        {
            // 오버랩 리스트 추가
            if (other.TryGetComponent<IActionTarget>(out IActionTarget item)) {
                if (!overlapItems.Contains(item)) { 
                    overlapItems.Add(item);
                }
            }
            Debug.Log($"시야 리스트 내 {overlapItems.Count}개");
            Debug.Log($"선정된 아이템 : {ClosestItem()}");
        }
    }

    // 시야콜라이더에서 벗어났을 때 리스트에서 삭제
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item") || other.CompareTag("Deactive")) {
            // 윤곽선 삭제
            if (other.TryGetComponent<Outline>(out Outline outline))
            {
                outline.enabled = false;
            }
            if (other.TryGetComponent<IActionTarget>(out IActionTarget item)) { 
                overlapItems.Remove(item);
            }
        }
    }

    // 바라보고 있는 오브젝트 중 가장 가까운 오브젝트 검출
    public IActionTarget ClosestItem()
    {
        Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
        float minDistance = float.MaxValue;
        IActionTarget closestItem = null;

        if (overlapItems.Count == 1)
        {
            return overlapItems[0];
        }
        else if (overlapItems.Count == 0)
        {
            return null;
        }
        else
        {
            Ray ray = mainCam.ScreenPointToRay(center);
            if (Physics.Raycast(ray, out RaycastHit hit, 4f))
            {
                // 레이에 맞은 콜라이더가 IActionItem인지 확인
                IActionTarget item = hit.collider.GetComponent<IActionTarget>();
                if (item != null && overlapItems.Contains(item))
                {
                    return item;
                }
            }

            // raycast로 감지된게 없다면, 기존 방식
            foreach (var item in overlapItems)
            {
                var converseItem = item as MonoBehaviour;
                if (converseItem != null)
                {
                    Vector3 worldPos = converseItem.transform.position;
                    Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);

                    float distance = Vector2.Distance(new Vector2(screenPos.x, screenPos.y), center);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestItem = item;
                    }
                }
            }
            return closestItem;
        }
    }



    // 아이템 획득 시 아이템 시야 List에서 삭제
    private void ExceptItem(GameEvents.GetItem evt) { 
        
    }

    
}
