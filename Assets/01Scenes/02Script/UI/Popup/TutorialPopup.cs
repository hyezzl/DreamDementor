using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    private BlinkAnnounce blink;
    

    private void Awake()
    {
        if (!TryGetComponent<BlinkAnnounce>(out blink))
            Debug.Log("TutorialPopup - Failed to Load BlinkAnnounce");
    }

    private void OnEnable()
    {
        StartCoroutine(blink.BlinkAnnounceMSG(group));
    }
}
