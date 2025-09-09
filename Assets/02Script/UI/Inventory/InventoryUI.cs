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
    private const int slotCnt = 8;      // 기본값
    private List<InventorySlot> slots = new();
    private bool isOpen = false;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("InventoryUI - Failed to Load PlayerController");

        if (!TryGetComponent<InventoryDescription>(out desc)) {
            Debug.Log("InventoryUI - Failed to Load InventoryDescription");
        }

        // 슬롯 생성
        for (int i = 0; i < slotCnt; i++) { 
            var slot = Instantiate(slotPrefab, slotParent).GetComponent<InventorySlot>();
            slot.Init(DatabaseManager.Instance);
            slots.Add(slot);
        }
        //RefreshInventory();
    }

    //Temp
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            RefreshInventory();
        }
    }


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.InventoryChanged>(OnInventoryChanged);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.InventoryChanged>(OnInventoryChanged);
    }
    private void OnInventoryChanged(UIEvents.InventoryChanged evt) {
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
