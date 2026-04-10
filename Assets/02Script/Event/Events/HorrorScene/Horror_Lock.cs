using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Horror_Lock : MonoBehaviour
{
    [Header("MyFloor")]
    [SerializeField] private HorrorFloor myFloor;

    [Header("Answer")]
    [SerializeField] private int[] answer1F = { 4,7,9,8 };
    [SerializeField] private int[] answer2F = { 2,5,6,1 };

    [Header("Ref")]
    [SerializeField] private SorrowNumberSlot[] slots;
    [SerializeField] private Sprite[] leverSprites;
    [SerializeField] private Button leverBtn;
    [SerializeField] private Image leverImg;

    private int[] targetAnswer; 


    private void Start()
    {
        SetupAnswer();
        leverBtn?.onClick.AddListener(CheckAnswer);
    }


    private void CheckAnswer()
    {
        // 버튼 중복 클릭 방지 (필요 시)
        leverBtn.interactable = false;

        StartCoroutine(CheckSequence());
    }

    private void SetupAnswer()
    {
        // 인스펙터에서 설정한 myFloor 값에 따라 공동 answer 변수에 할당
        if (myFloor == HorrorFloor.Floor1)
            targetAnswer = answer1F;
        else if (myFloor == HorrorFloor.Floor2)
            targetAnswer = answer2F;

        if (targetAnswer == null || targetAnswer.Length != slots.Length)
            Debug.LogError($"{gameObject.name} : 정답 할당 실패");
    }


    private IEnumerator CheckSequence()
    {
        // sprite 변경

        yield return new WaitForSeconds(0.2f);

        // 정답 검사
        bool isCorrect = true;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].CurVal != targetAnswer[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            OnCorrect();
        }
        else
        {
            Debug.Log("오답!");
            OnFail();
            leverBtn.interactable = true;
        }
    }
    private void OnCorrect()
    {
        Debug.Log("정답! Next!");
        leverBtn.interactable = false;

        // 트리거존에 정답임을 알림
        EventBus.Instance.Publish<PuzzleEvents.H_Unlock>(new PuzzleEvents.H_Unlock(myFloor));

        // 퍼즐닫음
        EventBus.Instance.Publish<UIEvents.UIPuzzleComplete>(new UIEvents.UIPuzzleComplete());

        // 다음 층으로 이동!
        EventBus.Instance.Publish(new PuzzleEvents.ToNextFloor(myFloor));

    }

    private void OnFail()
    {
        // 레버 다시 돌아감
        
    }
}
