using UnityEngine;

public class Fountain : MonoBehaviour
{
    [SerializeField] private Renderer falling;
    [SerializeField] private Renderer particle;
    public Material fallingMT;
    public Material particleMT;
    private bool flipped = false;
    private float timer = 0f;
    public float flipInterval = 0.1f;

    private void Awake()
    {
        // 인스턴스 생성
        fallingMT = falling.material;
        particleMT = particle.material;
    }

    private void Start()
    {
        fallingMT.mainTextureScale = new Vector2(1, 1);
        fallingMT.mainTextureOffset = Vector2.zero;
        particleMT.mainTextureScale = new Vector2(1, 1);
        particleMT.mainTextureOffset = Vector2.zero;
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
                fallingMT.mainTextureScale = new Vector2(-1, 1);
                fallingMT.mainTextureOffset = new Vector2(1, 0);
                particleMT.mainTextureScale = new Vector2(-1, 1);
                particleMT.mainTextureOffset = new Vector2(1, 0);
            }
            else
            {
                fallingMT.mainTextureScale = new Vector2(1, 1);
                fallingMT.mainTextureOffset = Vector2.zero;
                particleMT.mainTextureScale = new Vector2(1, 1);
                particleMT.mainTextureOffset = Vector2.zero;
            }
        }
    }
}