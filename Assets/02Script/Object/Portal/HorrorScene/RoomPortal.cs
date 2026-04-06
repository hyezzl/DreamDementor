using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPortal : MonoBehaviour
{
    [SerializeField] private RoomStep rm;   // 단계관리 룸 매니저
    private BoxCollider col;

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
    }


    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 물체가 플레이어인가?
        if (other.CompareTag("Player")) {
            if (rm != null)
            {
                if (rm.curStep < 2)
                {
                    rm.ToNextStep(other.gameObject);

                    // 포탈 효과음

                    // 포탈 효과
                }
                else
                {
                    Debug.Log("포탈 비활성화");
                }
            }
        }
    }

    // 외부에서 포탈
}
