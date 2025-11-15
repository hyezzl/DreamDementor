using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WeatherManager : MonoBehaviour
{
    public GameObject weatherParticle;      // 날씨 파티클
    public GameObject tempParticle;         // 임시 파티클

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.ChangeWeather>(OnChangeWeather);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.ChangeWeather>(OnChangeWeather);
    }

    private void OnChangeWeather(GameEvents.ChangeWeather evt) {
        if (evt.isOn) {
            weatherParticle.SetActive(true);
        }
        else {
            weatherParticle.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 닿으면 날씨효과 OFF
        if (other.CompareTag("Player")) {
            weatherParticle.SetActive(false);

            // 이미 내려온상태로 켜짐
            tempParticle.SetActive(true);
            var ps = tempParticle.GetComponent<ParticleSystem>();
            if (ps != null) {
                ps.Simulate(ps.main.startLifetime.constant, true, false, true);
                ps.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 특정존(ex.터널) 나가면 날씨효과 ON
        if (other.CompareTag("Player"))
        {
            weatherParticle.SetActive(true);
            StartCoroutine(FadeOutTemp(tempParticle));
        }
    }

    private IEnumerator FadeOutTemp(GameObject obj) {
        yield return new WaitForSeconds(2f);

        if (obj != null)
        {
            obj.SetActive(false);
        }
    }
}
