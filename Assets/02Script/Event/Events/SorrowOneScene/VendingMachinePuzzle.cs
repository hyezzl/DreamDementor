using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachinePuzzle : MonoBehaviour
{
    private PuzzleManager pm;

    // 두가지 버전의 자판기
    public GameObject uncompleteVendingMachine;
    public GameObject completeVendingMachine;

    // 두개의 버튼
    public Button selectBtn;
    public Button getBtn;

    // 메모 애니메이터
    public Animator memoAnim;

    private void Awake()
    {
        pm = transform.parent.GetComponent<PuzzleManager>();
        if (pm == null) Debug.Log("VendingMachinePuzzle - failed to Load PuzzleManager");
    }

    private void Start()
    {
        selectBtn.gameObject.SetActive(false);
        getBtn.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PuzzleEvents.SO_WorkVendingMachine>(CompleteVendingMachine);

        selectBtn.onClick.AddListener(OnClickMemoButton);
        getBtn.onClick.AddListener(OnGetMemo);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PuzzleEvents.SO_WorkVendingMachine>(CompleteVendingMachine);

        selectBtn.onClick.RemoveListener(OnClickMemoButton);
        getBtn.onClick.RemoveListener(OnGetMemo);
    }


    private void CompleteVendingMachine(PuzzleEvents.SO_WorkVendingMachine evt) {
        // 두개의 동전이 모두 들어갔을때
        Debug.Log("자판기 퍼즐 완료!");

        // 자판기 변경
        completeVendingMachine.SetActive(true);
        uncompleteVendingMachine.SetActive(false);

        // 효과음

        // 메모 버튼 활성화
        selectBtn.gameObject.SetActive(true);


        // Close
        //pm.CloseAllPuzzleUI();
        //EventBus.Instance.Publish<PuzzleEvents.CutOff>(new PuzzleEvents.CutOff(60001003));
    }

    // 메모 버튼 눌렸을 때
    private void OnClickMemoButton() 
    {
        // 애니메이션 발생
        if (memoAnim != null) { 
            memoAnim.SetTrigger("FallDown");

            StartCoroutine(MemoAnimation());
        }
    }

    // 메모 습득 버튼 눌렸을 때
    private void OnGetMemo() {
        // 기본 UI내려감

        // 메모 UI 켜짐
        Debug.Log("메모UI켜짐");

    }

    IEnumerator MemoAnimation() { 
        yield return null;

        yield return new WaitForSeconds(2f);

        // 메모 습득 버튼 활성화
        getBtn.gameObject.SetActive(true);
        selectBtn.gameObject.SetActive(false);
    }
}

