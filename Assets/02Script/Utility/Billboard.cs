using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void LateUpdate()
    {
        //transform.forward = Camera.main.transform.forward; // 빌보드
        Vector3 targetPosition = Camera.main.transform.position;
        targetPosition.y = transform.position.y; // Y축은 고정
        transform.LookAt(targetPosition);
    }
}
