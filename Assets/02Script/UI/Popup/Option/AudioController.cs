using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum SoundType
{ 
    Master,
    SFX,
    BGM
}

public class AudioController : MonoBehaviour
{
    [Header("AudioMixer")]
    [SerializeField] private AudioMixer am;

    [Header("UIRefs")]
    [SerializeField] private List<Button> buttons;
    [SerializeField] private List<Image> images;
    [SerializeField] private List<Slider> sliders;
    [SerializeField] private List<TextMeshProUGUI> texts;

    [Header("Icons")]
    [SerializeField] private Sprite playIcon;
    [SerializeField] private Sprite muteIcon;

    private List<bool> isPlaying = new List<bool> { true, true, true };
    private List<float> preVol = new List<float> { 1f, 1f, 1f };

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            sliders[i].value = 1f;
            preVol[i] = 1f;
            isPlaying[i] = true;

            texts[i].text = "100 %";
            images[i].sprite = playIcon;
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < 3; i++) {
            int idx = i;
            sliders[idx].onValueChanged.AddListener(value => SliderValueChanged((SoundType)idx, value));
            buttons[idx].onClick.AddListener(() => SoundMute((SoundType)idx));
        }
    }
    private void OnDisable()
    {
        foreach (var slider in sliders) slider.onValueChanged.RemoveAllListeners();
        foreach (var button in buttons) button.onClick.RemoveAllListeners();
    }

    public void SliderValueChanged(SoundType type, float value) {
        // 볼륨 텍스트 표시
        int percent = Mathf.RoundToInt(value * 100);
        texts[(int)type].text = percent + " %";

        // AudioMixer 볼륨 조절
        float db;   // -80 ~ 0
        if (value == 0)
        {
            // 음소거
            images[(int)type].sprite = muteIcon;
            db = -80f;
            isPlaying[(int)type] = false;
        }
        else
        {
            images[(int)type].sprite = playIcon;
            db = Mathf.Log10(value) * 20f;  // 데시벨
            isPlaying[(int)type] = true;
        }
        string parameter = type.ToString() + "Vol";
        am.SetFloat(parameter, db);
    }

    public void SoundMute(SoundType type) {
        // 현재 볼륨
        float curVal = sliders[(int)type].value;
        if (curVal > 0f)
        {
            // 캐싱
            preVol[(int)type] = curVal;
            isPlaying[(int)type] = false;
            sliders[(int)type].SetValueWithoutNotify(0f);
            images[(int)type].sprite = muteIcon;
            texts[(int)type].text = "0 %";
            SliderValueChanged(type, 0f);
        }
        else
        {
            // 복원
            isPlaying[(int)type] = true;
            sliders[(int)type].SetValueWithoutNotify(preVol[(int)type]);
            images[(int)type].sprite = playIcon;
            texts[(int)type].text = Mathf.RoundToInt(preVol[(int)type] * 100) + " %";
            SliderValueChanged(type, preVol[(int)type]);
        }
    }

}
