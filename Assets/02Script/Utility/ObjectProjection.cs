using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 3D 오브젝트용
/// </summary>

public class ObjectProjection : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float alphaRatio2D = 0.6f;
    [SerializeField] private float alphaRatio3D = 0.15f;
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
            // 플레이어 자신 무시
            if(hit.collider.gameObject == player.gameObject) continue;

            // 플레이어보다 뒤에 위치한 물체 무시
            if (hit.collider.transform.position.z > player.position.z) continue;

            Renderer rend = hit.collider.GetComponent<Renderer>();
            if(rend == null) continue;

            // Sprite Renderer 와 Mesh Renderer 분기
            if(rend is SpriteRenderer rend2D){
                Color color = rend2D.material.color;

                color.a = alphaRatio2D;
                rend2D.material.color = color;
                renderers.Add(rend2D);
            }
            else if(rend is MeshRenderer rend3D){
                Color color = rend3D.material.color;

                color.a = alphaRatio3D;
                rend3D.material.color = color;
                renderers.Add(rend3D);
            }
        }
    }
}
