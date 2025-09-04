using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temppp : MonoBehaviour
{
    [SerializeField] private Renderer target1;
    [SerializeField] private Renderer target2;
    public Material material1;
    public Material material2;
    private bool flipped = false;
    private float timer = 0f;
    public float flipInterval = 0.8f; // 반전 주기 (초)

    private void Start()
    {
        // 인스턴스화하여 원본 머티리얼 피해요
        //material1 = target1.material;
        //material2 = target1.material;
        // 초기 세팅 (필요 시)
        material1.mainTextureScale = new Vector2(1, 1);
        material1.mainTextureOffset = Vector2.zero;
        material2.mainTextureScale = new Vector2(1, 1);
        material2.mainTextureOffset = Vector2.zero;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= flipInterval)
        {
            timer = 0f;
            flipped = !flipped;

            if (flipped)
            {
                Debug.Log(flipped);
                material1.mainTextureScale = new Vector2(-1, 1);
                material1.mainTextureOffset = new Vector2(1, 0);
            }
            else
            {
                Debug.Log(flipped);

                material1.mainTextureScale = new Vector2(1, 1);
                material1.mainTextureOffset = Vector2.zero;
            }
        }
    }
}
