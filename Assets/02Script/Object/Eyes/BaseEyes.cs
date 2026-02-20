using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEyes : MonoBehaviour
{
    public Sprite[] eyes;
    public Transform player;
    protected SpriteRenderer sr;

    protected virtual void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) Debug.Log("BaseEyes - Failed to Load SpriteRenderer");
    }

    protected virtual void Update() {
        if (player != null && sr != null) { 
            UpdateEyeVer();
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



}
