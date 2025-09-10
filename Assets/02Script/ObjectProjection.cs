using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectProjection : MonoBehaviour
{
    [SerializeField] private Transform player;

    private PlayerController pc;
    private List<Renderer> hits = new List<Renderer>();

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
    private void OnChange(GameEvents.GameModeChange evt) { 
        // 3인칭일 경우에만 Projection


    }

    private void ReturnObject() {
        foreach (Renderer rend in hits) {
            Color color = rend.material.color;
            color.a = 1f; // 되돌림
            rend.material.color = color;
        }
        hits.Clear();

        // 카메라에서 캐릭터로 레이캐스트
    }
}
