using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownBTN : MonoBehaviour
{
    [Header("UI Ref")]
    [SerializeField] private Image blackOverlayImage; // 쿨타임 보여줄 검은 오버레이 이미지
    [SerializeField] private Button targetButton;     // 실제 클릭할 버튼

    [SerializeField] private float cooldownDuration = 3f;

    private void OnEnable()
    {
        ResetCooldown();
        StartCoroutine(FillCooldown());
    }

    private void ResetCooldown()
    {
        targetButton.interactable = false;
        blackOverlayImage.fillAmount = 1f;  // 검은색이 완전히 덮은 상태
    }


    private IEnumerator FillCooldown()
    {
        float timer = 0f;
        while (timer < cooldownDuration)
        {
            timer += Time.deltaTime;
            float fillValue = 1f - (timer / cooldownDuration); // 점점 줄어들게

            blackOverlayImage.fillAmount = fillValue;

            yield return null;
        }

        blackOverlayImage.fillAmount = 0f;  // 완전히 비워짐
        Color finalColor = blackOverlayImage.color;
        finalColor.a = 0f;  // 완전 투명
        blackOverlayImage.color = finalColor;

        targetButton.interactable = true;   // 버튼 활성화
    }
}
