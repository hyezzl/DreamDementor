using System.Collections;
using UnityEngine;

public class BlinkAnnounce : MonoBehaviour
{
    [SerializeField] private float term;
    [SerializeField] private float maxAlpha;

    public IEnumerator BlinkAnnounceMSG(CanvasGroup group, float term = 2) {
        float t = 0;
        bool fadeIn = true;
        while (true) {
            t += Time.deltaTime * term;
            group.alpha = fadeIn ? Mathf.Lerp(0f, maxAlpha, t) : Mathf.Lerp(maxAlpha, 0f, t);

            if (t >= 1f) {
                t = 0f;
                fadeIn = !fadeIn;
            }
            yield return null;
        }
    }
}
