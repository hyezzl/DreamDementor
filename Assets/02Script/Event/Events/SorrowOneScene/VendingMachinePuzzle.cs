using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachinePuzzle : MonoBehaviour
{
    // 두개의 드랍 존
    private PuzzleManager pm;

    private void Awake()
    {
        pm = transform.GetComponent<PuzzleManager>();
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

        // 드랍존 검증 x2

        //효과음

    }
}
