using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_PuzzleManager : MonoBehaviour
{
    [SerializeField] private int curCoin;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PuzzleEvents.SO_GetCoin>(OnGetCoin);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PuzzleEvents.SO_GetCoin>(OnGetCoin);
    }

    private void OnGetCoin(PuzzleEvents.SO_GetCoin evt) {
        curCoin++;

        if (curCoin >= 2) {
            Debug.Log("자판기 사용 가능");

            EventBus.Instance.Publish<PuzzleEvents.SO_SetMachine>(new PuzzleEvents.SO_SetMachine());
        }
    }
}
