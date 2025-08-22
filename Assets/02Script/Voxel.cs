using UnityEngine;


/// <summary>
/// 랜덤한 방향으로 날아가는 복셀 (일정시간 지나면 삭제)
/// </summary>
public class Voxel : MonoBehaviour
{
    public float speed = 5f;
    public float destroyTime = 3f;
    float curTime;

    private void Start()
    {
        Vector3 direction = Random.insideUnitSphere;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = direction * speed;
    }
}
