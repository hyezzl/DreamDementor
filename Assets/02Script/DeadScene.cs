using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadScene : MonoBehaviour
{
    [SerializeField] private GameObject topBack;
    [SerializeField] private GameObject bottomBack;
    [SerializeField] private Image bloodSpot1;
    [SerializeField] private Image bloodSpot2;
    [SerializeField] private float fadeDuration = 3f;


    private void Start()
    {
        topBack.SetActive(false);
        bottomBack.SetActive(false);

        bloodSpot1.enabled = false;
        bloodSpot2.enabled = false;
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.GameOver>(OnOver);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GameOver>(OnOver);
    }

    private void OnOver(GameEvents.GameOver evt) {
        // 배경
        topBack.SetActive(true);
        bottomBack.SetActive(true);

        StartCoroutine(BloodEffect());
    }

    private IEnumerator BloodEffect() { 
        yield return null;
        float elapsed = 0f;

        bloodSpot1.enabled = true;
        //효과음

        yield return new WaitForSeconds(0.5f);

        bloodSpot2.enabled = true;
        //효과음

        float startAlpha1 = bloodSpot1.color.a;
        float startAlpha2 = bloodSpot2.color.a;

        while (elapsed < fadeDuration) { 
            elapsed += Time.deltaTime;
            float alpha1 = Mathf.Lerp(startAlpha1, 0f, elapsed / fadeDuration);
            float alpha2 = Mathf.Lerp(startAlpha2, 0f, elapsed / fadeDuration);

            bloodSpot1.color = new Color(bloodSpot1.color.r, bloodSpot1.color.g, bloodSpot1.color.b, alpha1);
            bloodSpot2.color = new Color(bloodSpot2.color.r, bloodSpot2.color.g, bloodSpot2.color.b, alpha2);
            yield return null;
        }
        bloodSpot1.color = new Color(bloodSpot1.color.r, bloodSpot1.color.g, bloodSpot1.color.b, 0f);
        bloodSpot2.color = new Color(bloodSpot2.color.r, bloodSpot2.color.g, bloodSpot2.color.b, 0f);
    }
}
