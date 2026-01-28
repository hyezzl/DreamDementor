using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachinePuzzle : MonoBehaviour
{
    private PuzzleManager pm;

    // 두가지 버전의 자판기
    public GameObject uncompleteVendingMachine;
    public GameObject completeVendingMachine;

    private void Awake()
    {
        pm = transform.parent.GetComponent<PuzzleManager>();
        if (pm == null) Debug.Log("VendingMachinePuzzle - failed to Load PuzzleManager");
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PuzzleEvents.SO_WorkVendingMachine>(CompleteVendingMachine);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PuzzleEvents.SO_WorkVendingMachine>(CompleteVendingMachine);
    }


    private void CompleteVendingMachine(PuzzleEvents.SO_WorkVendingMachine evt) {
        // 두개의 동전이 모두 들어갔을때
        Debug.Log("자판기 퍼즐 완료!");

        // 자판기 변경
        completeVendingMachine.SetActive(true);
        uncompleteVendingMachine.SetActive(false);

        //효과음

        // Close
        //pm.CloseAllPuzzleUI();

        //EventBus.Instance.Publish<PuzzleEvents.CutOff>(new PuzzleEvents.CutOff(60001003));

    }
}
