using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HappyDoorRing : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("DoorRing")]
    [SerializeField] private Button ringBTN;
    [SerializeField] private Image downImg;
    [SerializeField] private Image upImg;

    public HappyPuzzleManager pm;

    private bool isPointerInside = false;
    public bool IsUp { get; private set; } = false;

    private void Awake()
    {
        // 초기값
        ResetRing();

        if (pm == null) { 
            pm = FindAnyObjectByType<HappyPuzzleManager>();
        }
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerInside = true;
        SetRing(true);
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPointerInside)
        {
            SetRing(false);
            // 클릭 완료 시 실행할 함수 호출
            OnClicked();
        }
        else
        {
            ResetRing();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
        SetRing(false);
    }

    private void ResetRing()
    {
        SetRing(false);
    }

    private void SetRing(bool isUp) {
        this.IsUp = isUp;
        downImg.enabled = !isUp;
        upImg.enabled = isUp;
    }

    private void OnClicked() {
        // 퍼즐 검사 로직
        Debug.Log("문고리 클릭!");

        pm.TouchDoorRing();
    }
}
