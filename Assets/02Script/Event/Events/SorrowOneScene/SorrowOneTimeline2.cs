using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SorrowOneTimeline2 : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private CanvasGroup playerDialog;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI playerSpeaker;


    private void SetCanvasGroup2(CanvasGroup group, bool isActive)
    {
        group.alpha = isActive ? 1f : 0f;
        group.interactable = isActive;
        group.blocksRaycasts = isActive;
    }


    // 플레이어 대사
    public void SO_PlayerDialog2(string playerD)
    {
        SetCanvasGroup2(playerDialog, true);
        playerText.text = playerD;
        playerSpeaker.text = "서린";
    }

    public void OO_ClosePD2()
    {
        SetCanvasGroup2(playerDialog, false);
        playerText.text = "";
        playerSpeaker.text = "";
    }
}
