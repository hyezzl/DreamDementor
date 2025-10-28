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
        // SoundManager의 볼륨값으로 초기화
        //sliders[(int)SoundType.Master].value = SoundManager.Instance.masterVol;
        //sliders[(int)SoundType.BGM].value = SoundManager.Instance.bgmVol;
        //sliders[(int)SoundType.SFX].value = SoundManager.Instance.sfxVol;


        //for (int i = 0; i < sliders.Count; i++)
        //{
        //    float val = sliders[i].value;
        //    preVol[i] = val;
        //    isPlaying[i] = val > 0f;

        //    texts[i].text = Mathf.RoundToInt(val * 100) + " %";
        //    images[i].sprite = isPlaying[i] ? playIcon : muteIcon;
        //}
    }

    private void OnEnable()
    {
        InitializeSliders();

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

        // SoundManager와 상태 동기화
        switch (type) 
        {
            case SoundType.Master:
                SoundManager.Instance.SetMasterVol(value);
                break;

            case SoundType.BGM:
                SoundManager.Instance.SetBGMVol(value);
                break;

            case SoundType.SFX:
                SoundManager.Instance.SetSFXVol(value);
                break;
        }

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


    //temp
    public void InitializeSliders()
    {
        // SoundManager가 준비된 상태에서 호출되어야 함
        if (SoundManager.Instance == null) Debug.LogError("매니저가 널");
        sliders[(int)SoundType.Master].value = SoundManager.Instance.masterVol;
        sliders[(int)SoundType.BGM].value = SoundManager.Instance.bgmVol;
        sliders[(int)SoundType.SFX].value = SoundManager.Instance.sfxVol;

        for (int i = 0; i < sliders.Count; i++)
        {
            float val = sliders[i].value;
            preVol[i] = val;
            isPlaying[i] = val > 0f;

            texts[i].text = Mathf.RoundToInt(val * 100) + " %";
            images[i].sprite = isPlaying[i] ? playIcon : muteIcon;
        }
    }

}
