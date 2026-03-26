using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 특정 맵에서만 적용됨
public class FlashLight : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private PlayerMove pm;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private GameObject light;

    private void Start()
    {
        pm = GetComponent<PlayerMove>();
        if (pm == null) Debug.Log("FlashLight");
    }

    private void Update()
    {
        UpdateLightDir();
    }

    private void UpdateLightDir() {
        
        // 매 프레임 방향 갱신
        Vector3 dir = pm.PreDir;

        float rotationY = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

        Quaternion rotate = Quaternion.Euler(46.42f, rotationY, 0);

        // 부드러운 회전
        light.transform.rotation = Quaternion.Slerp(light.transform.rotation, rotate, Time.deltaTime * rotationSpeed);
    }
}
