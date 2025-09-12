using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIApps : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private List<Button> buttons;  // 순서대로


    [Header("Tabs")]
    [SerializeField] private List<RectTransform> tabs;

    private void OnEnable()
    {
        for (int i = 0; i < buttons.Count; i++) {
        }
    }
    private void OnDisable()
    {
        
    }

    // 세로 > 가로 애니메이션 후 실행

    //private IEnumerator OpenSetting() { 
        
    //}
}
