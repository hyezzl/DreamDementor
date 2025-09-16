using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIApp
{ 
    Inventory   = 0,
    Option      = 1,
    Memo        = 2,
    Status      = 3,
}


public class UIApps : MonoBehaviour
{
    [Header("Groups")]
    [SerializeField] private CanvasGroup verGrp;
    [SerializeField] private CanvasGroup horGrp;
    [SerializeField] private Image background;      // 임시?
    
    [Header("Buttons")]
    [SerializeField] private List<Button> buttons;  // 순서대로
    [SerializeField] private Button exitBTN;
    [SerializeField] private Button backBTN;


    [Header("Tabs")]
    [SerializeField] private List<GameObject> tabs;

    private int curTab = -1;     //캐싱

    private void Start()
    {
        OnOffgroup(verGrp, false);
        OnOffgroup(horGrp, false);
    }


    // Temp 내맘대로
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape)) {
        //    if (verGrp.alpha > 0f)
        //    {
        //        OnOffgroup(verGrp, false);
        //        background.enabled = false;
        //    }
        //    else if (horGrp.alpha > 0f) { 
        //        OnOffgroup(horGrp, false);
        //        background.enabled = false;
        //    }
        //}
    }



    private void OnEnable()
    {
        for (int i = 0; i < buttons.Count; i++) {
            int idx = i;
            buttons[idx].onClick.AddListener(() => OnClicked(idx));
        }

        backBTN.onClick.AddListener(() => StartCoroutine(HorToVer()));
        exitBTN.onClick.AddListener(EndGame);
    }
    private void OnDisable()
    {
        foreach (var BTN in buttons) BTN.onClick.RemoveAllListeners();
        backBTN.onClick.RemoveAllListeners();
        exitBTN.onClick.RemoveListener(EndGame);
    }


    private void OnClicked(int idx) {
        curTab = idx;
        Debug.Log($"{idx} 번째 탭 열림");

        StartCoroutine(TabChange());
    }


    // 세로 > 가로
    private IEnumerator VerToHor() {
        yield return null;
        OnOffgroup(verGrp, false);
        // 세로>가로 애니메이션
        // 애니메이션 끝나면
        OnOffgroup(horGrp, true);
    }

    private IEnumerator HorToVer() {
        yield return null;
        OnOffgroup(horGrp, false);
        OnOffgroup(verGrp, true);
    }

    // 애니메이션 끝난 후 탭 체인지
    private IEnumerator TabChange() {
        yield return StartCoroutine(VerToHor());
        // 애니메이션이 끝난 후
        if (curTab >= 0) { 
            OpenTab(curTab);
            curTab = -1;
        }
    }


    // 단순 탭체인지 함수
    private void OpenTab(int idx) {
        foreach (var tab in tabs) { tab.gameObject.SetActive(false); }

        tabs[idx].gameObject.SetActive(true);
    }

    // CanvasGroup On/Off
    private void OnOffgroup(CanvasGroup group, bool isOn) {
        group.alpha = isOn ? 1 : 0;
        group.interactable = isOn;
        group.blocksRaycasts = isOn;
    }

    // 게임종료
    private void EndGame() {
        Debug.Log("Really? 게임종료");
    }
}
