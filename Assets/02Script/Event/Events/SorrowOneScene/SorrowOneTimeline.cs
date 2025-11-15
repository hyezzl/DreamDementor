using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SorrowOneTimeline : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private CanvasGroup playerDialog;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI playerSpeaker;

    [SerializeField] private CanvasGroup enemyDialog;
    [SerializeField] private TextMeshProUGUI enemyText;
    [SerializeField] private TextMeshProUGUI enemySpeaker;

    // 임시 대사
    private string playerD = "이게 무슨 소리지..?";
    private string enemyD = "ㄷ...ㅏ....ㄴ....ㅔ.....ㅈ...ㅏ...ㄹ...ㅁ...ㅗ..ㅅ..ㅇ....ㅑ...";


    private void SetCanvasGroup(CanvasGroup group, bool isActive)
    {
        group.alpha = isActive ? 1f : 0f;
        group.interactable = isActive;
        group.blocksRaycasts = isActive;
    }


    // 플레이어 대사
    public void SO_PlayerDialog() {
        SetCanvasGroup(playerDialog, true);
        playerText.text = playerD;
        playerSpeaker.text = "서린";
    }

    public void OO_ClosePD() {
        SetCanvasGroup(playerDialog, false);
        playerText.text = "";
        playerSpeaker.text = "";
    }

    public void SO_EnemyDialog() {
        SetCanvasGroup(enemyDialog, true);
        enemyText.text = enemyD;
        enemySpeaker.text = "???";
    }

    public void OO_CloseED() {
        SetCanvasGroup(enemyDialog, false);
        enemyText.text = "";
        enemySpeaker.text = "";
    }

}
