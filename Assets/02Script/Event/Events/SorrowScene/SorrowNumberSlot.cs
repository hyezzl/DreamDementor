using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SorrowNumberSlot : MonoBehaviour
{
    private TextMeshProUGUI numText;
    private Button upBtn;
    private int curVal = 0;

    public int CurVal => curVal;    // Getter

    private void Awake()
    {
        numText = GetComponentInChildren<TextMeshProUGUI>();
        if (numText == null) Debug.Log("SorrowNumberSlot - Failed to Load TextMeshPro");

        upBtn = GetComponentInChildren<Button>();
        if (upBtn == null) Debug.Log("SorrowNumberSlot - Failed to Load Button");
    }

    private void OnEnable()
    {
        upBtn.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        upBtn.onClick.RemoveListener(OnClick);
    }

    // 버튼 클릭
    private void OnClick() {
        // 0 부터 오름차순으로 순환
        curVal = (curVal + 1) % 10;

        UpdateUI();
    }

    private void UpdateUI() {
        if (numText != null) { 
            numText.text = curVal.ToString();
        }
    }
}
