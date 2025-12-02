using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TitleSetting : MonoBehaviour
{
    [SerializeField] private Button optionBtn;
    [SerializeField] private Button xBtn;
    [SerializeField] GameObject window;
    [SerializeField] Button outBG;

    private void Start()
    {
        window.SetActive(false);
    }

    private void OnEnable()
    {
        optionBtn.onClick.AddListener(OpenWindow);
        xBtn.onClick.AddListener(CloseWindow);
        outBG.onClick.AddListener(CloseWindow);
    }

    private void OnDisable()
    {
        optionBtn?.onClick.RemoveListener(OpenWindow);
        xBtn?.onClick.RemoveListener(CloseWindow);
        outBG?.onClick.RemoveListener(CloseWindow);

    }

    private void OpenWindow() {
        if (!window.activeSelf) { 
            window.SetActive (true);
        }
    }

    private void CloseWindow() {
        if (window.activeSelf) { 
            window.SetActive (false);
        }
    }

}
