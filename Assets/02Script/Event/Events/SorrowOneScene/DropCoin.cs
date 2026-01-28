using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropCoin : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Animator leanAnim;
    private int insertTriggerHash;  // 해시값 미리 계산

    public CoinDropZone dropzone;

    private CanvasGroup group;      // 상호작용 제어용
    private Image[] childImgs;

    private RectTransform rect;
    private Vector2 startPos;       // 정확한 시작 지점
    private bool isDrag = false;    // 현재 드래그 중인지

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;

        insertTriggerHash = Animator.StringToHash("Insert");

        group = GetComponent<CanvasGroup>();
        childImgs = GetComponentsInChildren<Image>();   // 자식들의 이미지 미리 캐싱
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        if (!isDrag) {
            isDrag = true;

            // 드래그 시작 시 상호작용 차단
            SetInteract(false); 
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (!isDrag) return;

        // 드래그 중 코인 위치 업데이트
        rect.position = eventData.position;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        SetInteract(true);

        // 정답존안에 들어갔는지 판정
        RectTransform zone = dropzone.GetComponent<RectTransform>();
        if (RectTransformUtility.RectangleContainsScreenPoint(
            zone,
            eventData.position,
            eventData.pressEventCamera
            ))
        {
            // 성공
            dropzone.HandleCoin(this);
        }
        else {
            // 실패
            ReturnCoin();
        }
    }


    // 정답일 때
    public void InsertCoin(Vector3 targetPos) {
        isDrag = false;
        SetInteract(false);

        // 드랍존 위치에 스냅
        transform.position = targetPos;

        // 효과음

        // 애니메이션 재생
        if (leanAnim != null) {
            //leanAnim.SetTrigger("Insert");
            leanAnim.SetTrigger(insertTriggerHash);

            // 애니메이션 재생 후 삭제
            StartCoroutine(DestroyCoin());
        }
    }

    // 애니메이션 재생 후 실행할 함수
    private IEnumerator DestroyCoin() {
        yield return new WaitForSeconds(0.05f);  // 0.05초 여유

        yield return new WaitForSeconds(0.833f);

        Destroy(gameObject);
    }



    // 오답일 때 위치 복구
    public void ReturnCoin() {
        isDrag = false;
        SetInteract(true);

        rect.anchoredPosition = startPos;
    }


    // 상호작용 제어 함수
    private void SetInteract(bool interactable) {
        if (group != null) {
            group.blocksRaycasts = interactable;
            group.interactable = interactable;
        }

        foreach (var img in childImgs) {
            if (img != null) {
                img.raycastTarget = interactable;
            }
        }
    }

}