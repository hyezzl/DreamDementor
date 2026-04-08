using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEyes : MonoBehaviour
{
    public Sprite[] eyes;
    public Transform player;

    protected string startEventID = "E060";     // 눈이 떠질 시점
    protected Animator anim;
    protected SpriteRenderer sr;
    protected string stateName;

    private bool isOpen = false;    // 눈이 떠졌는지?


    protected virtual void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.EndDialog>(AfterDialog);
    }
    protected virtual void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.EndDialog>(AfterDialog);
    }

    protected virtual void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) Debug.Log("BaseEyes - Failed to Load SpriteRenderer");

        if (anim == null) {
            anim = GetComponent<Animator>();
        }
    }

    protected virtual void LateUpdate() {
        if (isOpen && player != null && sr != null) {
            if (!IsBlinking()) {
                UpdateEyeVer();
            }
        }
    }


    protected abstract int GetVer(Vector3 dir);

    protected void UpdateEyeVer() {

        Vector3 dist = player.position - transform.position;
        dist.y = 0;     // y 원소는 필요없음

        int idx = GetVer(dist);
        idx = Mathf.Clamp(idx, 0, eyes.Length - 1);

        sr.sprite = eyes[idx];
    }

    protected void BlinkEye() {
        if (anim != null)
        {
            anim.SetTrigger("blink");
        }
    }

    protected IEnumerator RandomBlink() {
        yield return new WaitForSeconds(Random.Range(1f, 3f));

        while (isOpen) {
            float sec = Random.Range(5f, 10f);
            yield return new WaitForSeconds(sec);

            BlinkEye();
        }
    }

    // 현재 애니메이션 상태인지 조사
    protected bool IsBlinking() {
        if (anim == null ||
            string.IsNullOrEmpty(stateName)) return false;

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);  // 0번 레이어
        return state.IsName(stateName);
    }

    protected virtual void AfterDialog(UIEvents.EndDialog evt)
    {
        if (evt.eventID == startEventID)
        {
            StartCoroutine(EyeAnim());
        }
    }

    protected virtual IEnumerator EyeAnim() {
        yield return null;

        anim.SetTrigger("open");

        yield return new WaitForSeconds(1f);

        isOpen = true;
        StartCoroutine(RandomBlink());
    }



}
