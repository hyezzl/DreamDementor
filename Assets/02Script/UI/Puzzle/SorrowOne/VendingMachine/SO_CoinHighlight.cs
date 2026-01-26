using UnityEngine;
using UnityEngine.UI;

public class SO_CoinHighlight : MonoBehaviour
{
    public bool isBlink = true;
    public Image back;

    public float blinkspeed = 1.5f;
    private float maxAlpha = 0.58f;
    private float minAlpha = 0.105f;

    void Update()
    {
        if (isBlink) {
            Highlighting();
        }
    }

    private void Highlighting() {

        // 0 ~ maxAlpha ªÁ¿Ã ø’∫π
        float a = Mathf.PingPong(Time.time * blinkspeed, maxAlpha);

        Color color = back.color;
        color.a = a;
        back.color = color;
    }
}
