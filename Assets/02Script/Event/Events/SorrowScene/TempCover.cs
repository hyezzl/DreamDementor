using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCover : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] npcs;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어와 충돌 시, npc 활성화
            foreach (var npc in npcs) {
                npc.enabled = true;
            }
        }
    }
}
