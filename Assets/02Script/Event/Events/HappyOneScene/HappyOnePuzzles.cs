using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleEvents;

public class HappyOnePuzzles : MonoBehaviour
{
    public static int HOcurKeyCnt = 0;       // 현재 플레이어가 가진 열쇠 갯수

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PuzzleEvents.HO_GetKey>(OnGetKey);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PuzzleEvents.HO_GetKey>(OnGetKey);
    }



    private void OnGetKey(PuzzleEvents.HO_GetKey evt) {
        HOcurKeyCnt++;

        ManageKeyCnt(HOcurKeyCnt);
    }

    private void ManageKeyCnt(int curKeyCnt) {
        if (curKeyCnt == 2) {
            // key개수가 2개에 도달하면 귀신 등장
            Debug.Log("열쇠 두개 모았으므로 괴물이 등장!");
            EventBus.Instance.Publish<PuzzleEvents.HO_AppearEnemy>(new PuzzleEvents.HO_AppearEnemy());
        }
    }
}
