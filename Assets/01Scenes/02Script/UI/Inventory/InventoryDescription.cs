using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDescription : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private GameObject Description;
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

    public void Init(IDatabase db) {
        database = db;
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.SlotClicked>(OnSlotClicked);
        exitBTN?.onClick.AddListener(CloseDescription);
        Clear();
    }
    private void OnDisable()
    {
        EventBus.Instance.Subscribe<UIEvents.SlotClicked>(OnSlotClicked);
        exitBTN?.onClick.RemoveListener(CloseDescription);
    }

    private void OnSlotClicked(UIEvents.SlotClicked evt) {
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
}
