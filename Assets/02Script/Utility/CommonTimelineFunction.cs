using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommonTimelineFunction : MonoBehaviour
{
    [Header("Player UI Refs")]
    [SerializeField] private CanvasGroup playerDialog;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI playerSpeaker;

    [Header("Enemy UI Refs")]
    [SerializeField] private CanvasGroup enemyDialog;
    [SerializeField] private TextMeshProUGUI enemyText;
    [SerializeField] private TextMeshProUGUI enemySpeaker;

    // 말풍선 제어 함수 (UI)
    private void SetCanvasGroup(CanvasGroup group, bool isActive)
    {
        group.alpha = isActive ? 1f : 0f;
        group.interactable = isActive;
        group.blocksRaycasts = isActive;
    }


    /// <summary>
    /// 플레이어 대사
    /// </summary>
    /// <param name="playerD"></param>
    public void PlayerDialog(string playerD)
    {
        SetCanvasGroup(playerDialog, true);
        playerText.text = playerD.Replace("\\n", "\n");
        playerSpeaker.text = "서린";
    }

    /// <summary>
    /// 엄마 대사
    /// </summary>
    /// <param name="momD"></param>
    public void MomDialog(string momD)
    {
        SetCanvasGroup(playerDialog, true);
        playerText.text = momD;
        playerSpeaker.text = "엄마";
    }

    /// <summary>
    /// 귀신 대사
    /// </summary>
    /// <param name="enemyD"></param>
    public void EnemyDialog(string enemyD)
    {
        SetCanvasGroup(enemyDialog, true);
        enemyText.text = enemyD.Replace("\\n", "\n");
        enemySpeaker.text = "몽령";
    }

    /// <summary>
    /// 커스텀 함수1 (플레이어 박스용) 
    ///     ex) 서린:안녕
    /// </summary>
    public void CustomBasicDialog(string combinedText) 
    {
        string[] splitText = combinedText.Split(':');

        SetCanvasGroup(playerDialog, true);
        playerSpeaker.text = splitText[0].Trim();
        playerText.text = splitText[1].Trim();
    }

    /// <summary>
    /// 커스텀 함수2 (괴물 박스용)
    ///     ex) ??? : 메롱
    /// </summary>
    public void CustomEnemyDialog(string combinedText)
    {
        string[] splitText = combinedText.Split(':');

        SetCanvasGroup(enemyDialog, true);
        enemySpeaker.text = splitText[0].Trim();
        enemyText.text = splitText[1].Trim();
    }


    /// <summary>
    /// 베이직 대화박스 닫기
    /// </summary>
    public void ClosePlayerDialogbox()
    {
        SetCanvasGroup(playerDialog, false);
        playerText.text = "";
        playerSpeaker.text = "";
    }

    
    /// <summary>
    /// 괴물 대화박스 닫기
    /// </summary>
    public void CloseEnemyDialogbox()
    {
        SetCanvasGroup(enemyDialog, false);
        enemyText.text = "";
        enemySpeaker.text = "";
    }
}
