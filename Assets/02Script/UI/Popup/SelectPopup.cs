using UnityEngine;
using UnityEngine.UI;

public class SelectPopup : MonoBehaviour
{
    [SerializeField] private GameObject selectBox;
    [SerializeField] private Button[] BTNs;

    private bool standbyInput = false;
    private TutorialEvent te;
    private DialogPopup dp;
    private int selectVal;

    private void Awake()
    {
        te = FindAnyObjectByType<TutorialEvent>();
        if (te == null) Debug.Log("SelectPopup - Failed to Load TutorialEvent");
        dp = FindAnyObjectByType<DialogPopup>();
        if (dp == null) Debug.Log("SelectPopup - Failed to Load DialogPopup");
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.OpenSelectBox>(OnOpenSelectBox);

        // 버튼 등록
        for (int i = 0; i < BTNs.Length; i++) {
            int index = i;
            BTNs[i].onClick.AddListener(() => Choice(i));
        }
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.OpenSelectBox>(OnOpenSelectBox);

        foreach (var btn in BTNs) {
            btn.onClick.RemoveAllListeners();
        }
    }

    private void OnOpenSelectBox(UIEvents.OpenSelectBox evt) {
        // 게임모드 변경

        Debug.Log("똑똑..");

        selectBox.SetActive(true);
        standbyInput = true;

        // 버튼 눌림

    }

    private void Choice(int idx) {
        if (!standbyInput) return;

        selectVal = idx; // idx값
        te.OnChoice(selectVal); // 넘김
        selectBox.SetActive(false);
        standbyInput = false;

        // Dialog도 끔
        StartCoroutine(dp.ClosePanel());
    }
}
