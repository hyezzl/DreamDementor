using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

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

    private void OnEnable()
    {
        for (int i = 0; i < 3; i++) { 
            
        }
    }


}
