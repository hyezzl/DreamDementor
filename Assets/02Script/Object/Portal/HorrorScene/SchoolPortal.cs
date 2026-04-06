using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolPortal : MonoBehaviour
{
    [SerializeField] private SchoolStep sm;   // 단계관리 g학교 매니저
    private BoxCollider col;


    private void Awake()
    {
        col = GetComponent<BoxCollider>();
    }


    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 물체가 플레이어인가?
        if (other.CompareTag("Player"))
        {
            if (sm != null)
            {
                // 비밀번호 로직 추가될예정

                if (sm.curSchoolStep < 2)
                {
                    sm.ToNextStep(other.gameObject);

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
