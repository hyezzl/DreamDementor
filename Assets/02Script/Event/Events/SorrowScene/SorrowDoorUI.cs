using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SorrowDoorUI : MonoBehaviour
{
    [Header("Answer")]
    [SerializeField] private int[] answer = { 3, 7, 5 };

    [Header("Ref")]
    [SerializeField] private SorrowNumberSlot[] slots;
    [SerializeField] private Button lever;
    [SerializeField] private Animator leverAnim;


    private void Start()
    {
        lever?.onClick.AddListener(CheckAnswer);
    }


    //private void CheckAnswer() {

    //    if (leverAnim != null) {
    //        leverAnim.SetTrigger("check");    
    //    }

    //    bool isCorrect = true;

    //    // 레버 누르면 정답 검사
    //    for (int i = 0; i < slots.Length; i++) {
    //        if (slots[i].CurVal != answer[i]) {
    //            isCorrect = false;
    //            break;
    //        }
    //    }

    //    if (isCorrect)
    //    {
    //        Debug.Log("정답! 다음 씬으로 이동!");
    //        OnCorrect();
    //    }
    //    else {
    //        Debug.Log("오답!");
    //        OnFail();
    //    }
    //}


    private void CheckAnswer()
    {
        // 버튼 중복 클릭 방지 (필요 시)
        lever.interactable = false;

        StartCoroutine(CheckSequence());
    }

    private IEnumerator CheckSequence()
    {
        // 애니메이션 실행
        if (leverAnim != null)
        {
            leverAnim.SetTrigger("check");
        }

        yield return new WaitForSeconds(0.3f);

        // 정답 검사
        bool isCorrect = true;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].CurVal != answer[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            Debug.Log("정답! 다음 씬으로 이동!");
            OnCorrect();
        }
        else
        {
            Debug.Log("오답!");
            OnFail();
            // 오답일 경우 버튼을 다시 활성화
            lever.interactable = true;
        }
    }
    private void OnCorrect() { 
        // 씬이동
    }

    private void OnFail() {
        // 레버 다시 돌아감
        if (leverAnim != null) {
            leverAnim.SetTrigger("fail");
        }
    }
}
