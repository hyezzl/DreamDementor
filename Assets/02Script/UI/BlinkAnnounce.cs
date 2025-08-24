using System.Collections;
using UnityEngine;

public class BlinkAnnounce : MonoBehaviour
{
    [SerializeField] private float term = 0.9f;

    public IEnumerator BlinkAnnounceMSG(CanvasGroup group, float term) {
        float t = 0;
        bool fadeIn = true;
        while (true) {
            t += Time.deltaTime * term;
            group.alpha = fadeIn ? Mathf.Lerp(0f, 0.6f, t) : Mathf.Lerp(0.6f, 0f, t);

            if (t >= 1f) {
                t = 0f;
                fadeIn = !fadeIn;
            }
            yield return null;
        }
    }
}
