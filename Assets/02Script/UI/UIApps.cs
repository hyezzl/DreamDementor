using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIApp
{
    Inventory = 0,
    Option = 1,
    Memo = 2,
    Status = 3,
}


public class UIApps : MonoBehaviour
{
    [Header("Groups")]
    [SerializeField] private Image background;      // 임시?

    [Header("Buttons")]
    [SerializeField] private List<Button> buttons;  // 순서대로
    [SerializeField] private Button escapeBTN;
    [SerializeField] private Button exitBTN;
    [SerializeField] private Button backBTN;

    [Header("Tabs")]
    [SerializeField] private List<GameObject> tabs;
    [SerializeField] private List<Button> tabBTNs;

    [Header("Icons")]
    [SerializeField] private List<Sprite> tabIcons;


    private int curTab = -1;     //캐싱
    private PopupManager popupManager;

    private void Awake()
    {
        if (!TryGetComponent<PopupManager>(out popupManager)) Debug.Log("UIApps - Failed to Load PopupManager");
    }


    //private void OnEnable()
    //{
    //    for (int i = 0; i < buttons.Count; i++)
    //    {
    //        int idx = i;
    //        buttons[idx].onClick.AddListener(() => OnClicked(idx));
    //    }

    //    backBTN.onClick.AddListener(() => StartCoroutine(HorToVer()));
    //    exitBTN.onClick.AddListener(ExitPopup);
    //    escapeBTN.onClick.AddListener(EndGame);
    //}
    private void OnEnable()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int idx = i;
            buttons[idx].onClick.AddListener(() =>
            {
                if (popupManager != null && popupManager.isAnimating) return; // 애니메이션 중엔 입력 무시
                OnClicked(idx);
            });
        }

        backBTN.onClick.AddListener(() => {
            if (popupManager == null || !popupManager.isAnimating)
                StartCoroutine(HorToVer());
        });
        exitBTN.onClick.AddListener(() => {
            if (popupManager == null || !popupManager.isAnimating)
                ExitPopup();
        });
        escapeBTN.onClick.AddListener(() => {
            if (popupManager == null || !popupManager.isAnimating)
                EndGame();
        });
    }


    private void OnDisable()
    {
        foreach (var BTN in buttons) BTN.onClick.RemoveAllListeners();
        backBTN.onClick.RemoveAllListeners();
        exitBTN.onClick.RemoveAllListeners();
        escapeBTN.onClick.RemoveListener(EndGame);
    }


    private void OnClicked(int idx)
    {
        curTab = idx;
        UIApp app = (UIApp)idx;

        if (popupManager != null && popupManager.isAnimating) return;
        if (popupManager != null) 
            popupManager.ShowHorizontal();

        StartCoroutine(TabChange());
    }


    // 세로 > 가로
    private IEnumerator VerToHor()
    {
        yield return null;
        // 세로>가로 애니메이션
        // 애니메이션 끝나면
    }

    private IEnumerator HorToVer()
    {
        if (popupManager != null && popupManager.isAnimating) yield break;
        
        yield return new WaitForSeconds(0.2f);

        if (popupManager != null)
        {
            popupManager.ShowVertical();
            popupManager.isAnimating = false;
        }
    }

    // 애니메이션 끝난 후 탭 체인지
    private IEnumerator TabChange()
    {
        yield return StartCoroutine(VerToHor());
        // 애니메이션이 끝난 후
        if (curTab >= 0)
        {
            OpenTab(curTab);
            curTab = -1;
        }
    }


    // 단순 탭체인지 함수
    private void OpenTab(int idx)
    {
        if (popupManager != null && popupManager.isAnimating) return; // 애니메이션 중 취소

        foreach (var tab in tabs) { tab.gameObject.SetActive(false); }
        tabs[idx].gameObject.SetActive(true);

        UpdateTabBTN(idx);

        UIApp app = (UIApp)idx;
        switch (app)
        {
            case UIApp.Inventory:
                EventBus.Instance.Publish<UIEvents.OpenInventory>(new UIEvents.OpenInventory());
                break;
        }
    }

    // CanvasGroup On/Off
    private void OnOffgroup(CanvasGroup group, bool isOn)
    {
        group.alpha = isOn ? 1 : 0;
        group.interactable = isOn;
        group.blocksRaycasts = isOn;
    }

    // 게임종료
    private void EndGame()
    {
        if (popupManager != null && popupManager.isAnimating) return;
        Debug.Log("Really? 게임종료");
    }

    // 현재 선택된 탭을 제외한 3개탭 반환
    private List<int> OtherTabs(int selectedTab)
    {
        List<int> otherTabs = new();

        for (int i = 0; i < 4; i++)
        {
            if (i != selectedTab) otherTabs.Add(i);
        }
        return otherTabs;
    }

    private void UpdateTabBTN(int selectedTab)
    {
        if (popupManager != null && popupManager.isAnimating) return; // 애니 중 입력 무시

        // 선택탭 외 나머지탭 인덱스
        List<int> otherTabs = OtherTabs(selectedTab);

        // 탭 버튼에 다른 3개탭 아이콘/버튼 할당
        for (int i = 0; i < tabBTNs.Count; i++)
        {
            int tabIdx = otherTabs[i];

            // 탭 아이콘 변경
            tabBTNs[i].GetComponent<Image>().sprite = tabIcons[tabIdx];

            // 버튼에 역할 할당
            tabBTNs[i].onClick.RemoveAllListeners();
            tabBTNs[i].onClick.AddListener(() => OpenTab(tabIdx));
        }
    }

    private void ExitPopup() {
        if (popupManager != null && popupManager.isAnimating) return;

        popupManager.ClosePopup();
    }

}