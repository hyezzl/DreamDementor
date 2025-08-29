using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EyeOpeningBlur : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    private DepthOfField depthOfField;
    public float openSpeed = 3f;
    private float targetFocusDistance = 10f; // 선명해지는 목표 거리

    void Start()
    {
        postProcessVolume.profile.TryGetSettings(out depthOfField);
        depthOfField.focusDistance.value = 0.1f; // 초기 흐림 (눈 감은 상태)
    }

    void Update()
    {
        // 시간이 지나면서 점차 초점을 멀리로 잡아 선명해지게 조절
        depthOfField.focusDistance.value = Mathf.Lerp(depthOfField.focusDistance.value, targetFocusDistance, Time.deltaTime * openSpeed);
    }
}
