using UnityEngine;
using UnityEngine.EventSystems;

public class DragDown : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private float dragThreshhold = 100f;
    [SerializeField] private PhoneUIManager uiM;

    // 드래그 변수
    private Vector2 startPos;
    private bool isDragging = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        startPos = eventData.position;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        float dragDistance = eventData.position.y - startPos.y;

        if (dragDistance < -dragThreshhold) {
            // UI 닫기 호출
            if (uiM != null) {
                uiM.StartCoroutine(uiM.ClosePopupUI());
            }
            isDragging = false;
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}
