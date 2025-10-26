using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  1인칭 맵 내 손 애니메이션
/// </summary>
public class HandMotion : MonoBehaviour
{
    private Image img;
    private Animator anim;
    private PlayerController pc;


    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("HandMotion - Failed to Load PlayerController");
        if (!TryGetComponent<Animator>(out anim)) Debug.Log("HandMotion - Failed to Load Animator");
        if (!TryGetComponent<Image>(out img)) Debug.Log("HandMotion - Failed to Load Image");
    }

    private void Start()
    {
        if (pc.CurAspect != AspectMode.OnepersonMode)
        {
            Debug.Log("여기가 실행되나요??");
            // 3인칭일때
            anim.enabled = false;
        }
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.GameModeChange>(OnModeChange);
        EventBus.Instance.Subscribe<GameEvents.AspectChange>(OnAspectChange);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GameModeChange>(OnModeChange);
        EventBus.Instance.Unsubscribe<GameEvents.AspectChange>(OnAspectChange);
    }

    private void OnModeChange(GameEvents.GameModeChange evt)
    {
        if (evt.mode == GameMode.InspectMode)
        {

        }
        else
        {
            anim.SetBool("isMove", false);
        }
    }

    private void OnAspectChange(GameEvents.AspectChange evt)
    {
        if (evt.mode == AspectMode.OnepersonMode)
        {
            anim.enabled = true;
        }
        else
        {
            anim.enabled = false;
        }
    }
}