using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class HappyOneTimeline : MonoBehaviour
{

    [Header("Timeline Ref")]
    [SerializeField] private CanvasGroup playerTextBox;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI playerSpeaker;

    [Header("Filter Refs")]
    [SerializeField] private CameraFilterPack_FX_Glitch1 glitch;


    ////////////////타임라인 시그널 함수

    public void OOappearEnemy()
    {
        string OOspeaker = "서린";
        playerTextBox.alpha = 1f;

        playerSpeaker.text = OOspeaker;

    }

    //public IEnumerator OOPlayDialog(string sentence, string speaker)
    //{
    //    Debug.Log("실행은됨?");
    //    SetCanvasGroup(playerTextBox, true);
    //    playerSpeaker.text = speaker;
    //    playerText.text = sentence;
    //    yield return null;
    //    //playerText.DOText(sentence, 0.1f);
    //    //yield return playerText.DOText(sentence, 0.1f);
    //    //yield return new WaitForSeconds(1.2f);
    //}

    public void OOcloseDialog()
    {
        playerTextBox.alpha = 0f; // 창닫기
        playerText.text = "";
        playerSpeaker.text = "";
    }

    private void SetCanvasGroup(CanvasGroup group, bool isActive)
    {
        group.alpha = isActive ? 1f : 0f;
        group.interactable = isActive;
        group.blocksRaycasts = isActive;
    }

    
    public void EnemyTransform()
    {
        if (glitch != null)
            glitch.Glitch = 1f;
    }
}
