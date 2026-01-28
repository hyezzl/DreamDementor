using UnityEngine;
using UnityEngine.UI;

public class SO_CoinHighlight : MonoBehaviour
{
    public bool isBlink = true;
    public Image back;

    public float blinkspeed = 1.2f;
    private float maxAlpha = 0.58f;
    private float minAlpha = 0.105f;

    void Update()
    {
        if (isBlink) {
            Highlighting();
        }
    }

    private void Highlighting() {

        // minAlpha ~ maxAlpha 사이 왕복
        float pingpingRange = maxAlpha - minAlpha;
        float a = minAlpha + Mathf.PingPong(Time.time * blinkspeed, pingpingRange);

        Color color = back.color;
        color.a = a;
        back.color = color;
    }

    // 외부에서 하이라이트 삭제
    public void StopHighlighting() { 
        back.gameObject.SetActive(false);
    }
}
