using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// 퍼즐을 드랍하는 존
/// 정답 아이콘만 드랍가능하며, 올바르면 부착
/// </summary>

public enum PuzzleItem
{
    None,
    KeyHead,
    KeyBody,
    KeyTail,

}

public class DropZone : MonoBehaviour, IDropHandler
{
    public PuzzleItem correctName;
    private bool isCorrect = false;      // 현재 맞춘 퍼즐인지


    // 외부에서 읽게힘
    public bool IsCorrect => isCorrect; 


    // 드랍됐을 때
    public void OnDrop(PointerEventData evt)
    {
        var dropped = evt.pointerDrag;
        if (dropped != null) { 
            // 드래그된 오브젝트에 Draggable 가져오기
            DraggableItem dragItem = dropped.GetComponent<DraggableItem>();
            if (dragItem != null && dragItem.itemName == correctName)
            {
                // 정답
                dropped.transform.SetParent(transform); // 드랍존 자식으로
                dropped.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

                isCorrect = true;
                Debug.Log("정답이므로 퍼즐 장착!");
            }
            else {
                isCorrect = false;
                // 오답
            }
        }
    }
}
