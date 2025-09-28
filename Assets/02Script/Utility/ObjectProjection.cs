using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 3D 오브젝트용
/// </summary>

public class ObjectProjection : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float alphaRatio = 0.6f;
    [SerializeField] private float sphereRadius = 1f;   // 
    [SerializeField] private LayerMask layer;

    private List<Renderer> renderers = new List<Renderer>();

    private PlayerController pc;

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
    private void ChangeAspect(GameEvents.AspectChange evt)
    {
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

    private void Transparency()
    {
        foreach (Renderer rend in renderers)
        {
            Color color = rend.material.color;
            color.a = 1f;
            rend.material.color = color;
        }
        renderers.Clear();

        Vector3 dir = player.position - transform.position;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, sphereRadius, 
                                                    dir.normalized, dir.magnitude, layer.value);

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null & hit.collider.gameObject != player.gameObject)
            {
                Color color = rend.material.color;

                color.a = alphaRatio;
                rend.material.color = color;
                renderers.Add(rend);
            }
        }
    }
}
