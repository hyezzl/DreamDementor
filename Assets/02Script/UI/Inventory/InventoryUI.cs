using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotParent;

    private PlayerController pc;
    private InventoryDescription desc;
    private IDatabase database;
    private const int slotCnt = 8;      // 기본값
    private List<InventorySlot> slots = new();
    private bool isOpen = false;

    public void Init(IDatabase db)
    {
        database = db;

        // 슬롯 생성
        if (slots.Count == 0) { 
            for (int i = 0; i < slotCnt; i++)
            {
                var slot = Instantiate(slotPrefab, slotParent).GetComponent<InventorySlot>();
                slot.Init(DatabaseManager.Instance);
                slots.Add(slot);
            }
        }
    }


    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("InventoryUI - Failed to Load PlayerController");

        if (!TryGetComponent<InventoryDescription>(out desc)) {
            Debug.Log("InventoryUI - Failed to Load InventoryDescription");
        }
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.InventoryChanged>(OnInventoryChanged);
        EventBus.Instance.Subscribe<UIEvents.OpenInventory>(OnOpenInventory);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.InventoryChanged>(OnInventoryChanged);
        EventBus.Instance.Unsubscribe<UIEvents.OpenInventory>(OnOpenInventory);
    }
    private void OnInventoryChanged(UIEvents.InventoryChanged evt) {
        RefreshInventory();
    }

    private void OnOpenInventory(UIEvents.OpenInventory evt) {
        RefreshInventory();
    }


    // 인벤토리 업데이트
    private void RefreshInventory() {
        var items = InventoryManager.Instance.GetInventory();
        Debug.Log($"인벤토리 업데이트 : 현재 총 {items.Count}개");

        // 슬롯에 인벤토리 정보 전달
        for (int i = 0; i < slots.Count; i++) {
            if (i < items.Count)
            {
                slots[i].Set(items[i]);
            }
            else
            {
                slots[i].Set(null);
            }
        }
    }

    private void ExitInventory() {
        if (!isOpen) return;

        // 게임모드 변경

        // desc창 비활성화
        desc.CloseDescription();
    }
}
