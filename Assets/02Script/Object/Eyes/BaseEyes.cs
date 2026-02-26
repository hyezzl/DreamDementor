using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEyes : MonoBehaviour
{
    public Sprite[] eyes;
    public Transform player;
    public Animator anim;
    protected SpriteRenderer sr;
    protected string stateName;

    protected virtual void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) Debug.Log("BaseEyes - Failed to Load SpriteRenderer");

        if (anim == null) {
            anim = GetComponent<Animator>();
        }

        StartCoroutine(RandomBlink());
    }

    protected virtual void LateUpdate() {
        if (player != null && sr != null) {

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
        while (true) {
            float sec = Random.Range(5f, 10f);
            yield return new WaitForSeconds(sec);

            BlinkEye();
            Debug.Log(gameObject.name + " 깜빡!");
        }
    }

    // 현재 애니메이션 상태인지 조사
    protected bool IsBlinking() {
        if (anim == null ||
            string.IsNullOrEmpty(stateName)) return false;

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);  // 0번 레이어
        return state.IsName(stateName);
    }



}
