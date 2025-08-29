using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeOpeningEffect : MonoBehaviour
{
    public RectTransform upperBackground;  // 위쪽 검은 배경
    public RectTransform lowerBackground;  // 아래쪽 검은 배경
    public RectTransform maskCircle;       // 중앙 동그란 마스크

    public float duration = 3f;

    private Vector2 upperStartPos;
    private Vector2 lowerStartPos;
    private Vector2 upperEndPos;
    private Vector2 lowerEndPos;

    private Vector3 maskStartScale;
    private Vector3 maskEndScale;

    // startcoroutine

    void Start()
    {
        // 시작 위치 저장 (중앙에 붙어있는 상태)
        upperStartPos = upperBackground.anchoredPosition;
        lowerStartPos = lowerBackground.anchoredPosition;

        // 위는 위쪽으로, 아래는 아래쪽으로 충분히 이동할 목표 위치 설정
        upperEndPos = upperStartPos + new Vector2(0, 600);  
        lowerEndPos = lowerStartPos + new Vector2(0, -600);

        // 마스크 초기 스케일 : x,z는 기본값 유지, y는 0으로 시작
        maskStartScale = new Vector3(maskCircle.localScale.x, 0f, maskCircle.localScale.z);
        // 목표 스케일 : 현재 크기 유지
        maskEndScale = maskCircle.localScale;

        // 초기 값 세팅
        maskCircle.localScale = maskStartScale;

        StartCoroutine(PlayEyeOpenAnimation());
    }

    IEnumerator PlayEyeOpenAnimation()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float backgroundT = Mathf.Clamp01(t * 1.5f);

            // 배경 위치 보간 (Lerp)
            upperBackground.anchoredPosition = Vector2.Lerp(upperStartPos, upperEndPos, backgroundT);
            lowerBackground.anchoredPosition = Vector2.Lerp(lowerStartPos, lowerEndPos, backgroundT);

            // 마스크 y 스케일만 보간, x와 z는 고정
            float newY = Mathf.Lerp(maskStartScale.y, maskEndScale.y, t);
            maskCircle.localScale = new Vector3(maskStartScale.x, newY, maskStartScale.z);

            yield return null;
        }

        // 애니메이션 끝난 후 위치 및 스케일 보정
        upperBackground.anchoredPosition = upperEndPos;
        lowerBackground.anchoredPosition = lowerEndPos;
        maskCircle.localScale = maskEndScale;
    }
}
