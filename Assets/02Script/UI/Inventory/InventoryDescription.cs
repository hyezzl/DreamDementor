using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDescription : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] public GameObject Description;
    [SerializeField] private Button outsideBG;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private CanvasGroup selection;
    [SerializeField] private Button exitBTN;
    [SerializeField] private Button yesBTN;
    [SerializeField] private Button noBTN;
    [SerializeField] private Sprite nothing;

    private IDatabase database;
    private PickableData pickable;
    private EatableData eatable;
    private PopupManager pm;

    public bool isOpen = false;    // 현재 열려있는지?

    public void Init(IDatabase db) {
        database = db;
    }

    private void Awake()
    {
        pm = FindAnyObjectByType<PopupManager>();
        if (pm == null) Debug.Log("InventoryDescription - Failed to Load PopupManager");
    }

    private void Update()
    {
        // ESC 눌렀을 때 자체 세부창만 닫음
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Description.activeSelf)
            {
                CloseDescription();
                // 여기서 return하면 이 프레임엔 인벤토리는 안 닫힘!
                return;
            }
        }
    }

    private void OnEnable()
    {
        isOpen = false;

        EventBus.Instance.Subscribe<UIEvents.SlotClicked>(OnSlotClicked);
        exitBTN?.onClick.AddListener(CloseDescription);

        // Y & N
        yesBTN.onClick.AddListener(YesBTN);
        noBTN.onClick.AddListener(CloseDescription);

        outsideBG?.onClick.AddListener(CloseDescription);

        Clear();
    }
    private void OnDisable()
    {
        EventBus.Instance.Subscribe<UIEvents.SlotClicked>(OnSlotClicked);
        exitBTN?.onClick.RemoveListener(CloseDescription);

        yesBTN.onClick.RemoveListener(YesBTN);
        noBTN.onClick.RemoveListener(CloseDescription);

        outsideBG?.onClick.RemoveListener(CloseDescription);
    }

    private void OnSlotClicked(UIEvents.SlotClicked evt) {
        isOpen = true;

        Description.SetActive(true);
        ShowInformation(evt.item);
    }

    private void ShowInformation(ItemInstance item) {
        if (item == null) return;

        // 데이터 가져오기
        switch (item.type) {
            case ItemType.Pickable:
                pickable = database.GetPickable(item.itemID);
                if (pickable == null) { 
                    Debug.Log($"{item.itemID} : 정보를 불러오지 못함");
                    return;
                }
                itemName.text = pickable.itemName;
                itemDescription.text = pickable.description;
                icon.sprite = pickable.icon;

                ShowSelection(false);

                break;

            case ItemType.Eatable:
                eatable = database.GetEatable(item.itemID);
                if (eatable == null)
                {
                    Debug.Log($"{item.itemID} : 정보를 불러오지 못함");
                    return;
                }
                itemName.text = eatable.itemName;
                itemDescription.text = eatable.description;
                icon.sprite = eatable.icon;

                ShowSelection(true);

                break;
        }
    }

    public void CloseDescription() {
        isOpen = false;
        Description.SetActive(false);
    }

    // 초기화
    private void Clear() {
        itemName.text = "";
        itemDescription.text = "";
        icon.sprite = nothing;
    }

    private void ShowSelection(bool isShow) {
        if (isShow)
        {
            selection.alpha = 1f;
            selection.interactable = true;
            selection.blocksRaycasts = true;
        }
        else {
            selection.alpha = 0f;
            selection.interactable = false;
            selection.blocksRaycasts = false;
        }
    }

    // yes & no 버튼 로직
    private void YesBTN() {
        // 먹는 로직 (Eatable 한정)
        // 아이템에 따른 정신력 로직

        if (eatable != null) {
            // ItemInstance 인벤토리에서 삭제
            EventBus.Instance.Publish<GameEvents.UseItem>(new GameEvents.UseItem(eatable.itemID));

            // 정신력
            int? mental = eatable.mental;
            if (mental != null && mental != 0) {
                EventBus.Instance.Publish<GameEvents.OnDamaged>(new GameEvents.OnDamaged(eatable.mental));
            }

            // UI창 닫힘
            CloseDescription();
            pm.ClosePopup();
        }
    }
}
