using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectProjection : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float alphaRatio = 0.8f;

    private PlayerController pc;
    private List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    private bool isThree = false;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("ObjectProjection - Failed to Load PlayerController");
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.AspectChange>(ChangeAspect);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.AspectChange>(ChangeAspect);

    }
    private void ChangeAspect(GameEvents.AspectChange evt) {
        // 3인칭일 경우에만 Projection
        switch (evt.mode)
        {
            case AspectMode.ThirdpersonMode:
                isThree = true;
                break;

            case AspectMode.OnepersonMode:
                isThree = false;
                break;
        }
    }

    private void Update()
    {
        Transparency();
    }

    private void Transparency() {
        foreach (SpriteRenderer sr in renderers) {
            Color color = sr.color;
            color.a = 1f; // 되돌림
            sr.color = color;
        }
        renderers.Clear();

        // 카메라에서 캐릭터로 레이캐스트
        Vector3 dir = player.position - transform.position;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, dir.normalized);

        foreach (RaycastHit hit in hits)
        {
            SpriteRenderer sr = hit.collider.GetComponent<SpriteRenderer>();
            if (sr != null && hit.collider.gameObject != player.gameObject) 
            {
                Color color = sr.color;
                color.a = alphaRatio;
                sr.color = color;
                renderers.Add(sr);
            }
        }
    }
}
