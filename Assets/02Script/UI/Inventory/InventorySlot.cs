using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IInventory, IPointerClickHandler
{
    [Header("UI Refs")]
    [SerializeField] private Image icon;
    [SerializeField] private GameObject slot;
    [SerializeField] private Sprite nothing;
    [SerializeField] private TextMeshProUGUI nameBox;

    private ItemInstance item;
    private IDatabase database;

    public void Init(IDatabase db)
    {
        database = db;
    }

    public void Set(ItemInstance itemInst) {
        item = itemInst;

        if (item != null && item.itemID != 0)
        {
            icon.enabled = true;
            nameBox.enabled = true;

            string itemName = "";
            switch (itemInst.type)
            {
                case ItemType.Pickable:
                    if (database == null) Debug.Log("database가 널!");

                    var dataP = database.GetPickable(itemInst.itemID);
                    icon.sprite = dataP.icon;
                    itemName = dataP.itemName;
                    if (dataP.icon == null)
                        Debug.Log($"{itemInst.itemID}의 아이콘 불러오지 못함!");
                    if (dataP.itemName == null)
                        Debug.Log($"{itemInst.itemID}의 이름 불러오지 못함!");
                    break;

                case ItemType.Eatable:
                    if (database == null) Debug.Log("database가 널!");

                    var dataE = database.GetEatable(itemInst.itemID);
                    icon.sprite = dataE.icon;
                    itemName = dataE.itemName;
                    if (dataE.icon == null)
                        Debug.Log($"{itemInst.itemID}의 아이콘 불러오지 못함!");
                    if (dataE.itemName == null)
                        Debug.Log($"{itemInst.itemID}의 이름 불러오지 못함!");
                    break;
            }
            nameBox.text = itemName;
        }
        else {
            icon.enabled = false;
            icon.sprite = nothing;
            nameBox.enabled = false;
            nameBox.text = "";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        // 빈슬롯은 클릭 이벤트 무시
        if (item == null || item.itemID == 0) return;

        // 슬롯이 클릭!
        Debug.Log("슬롯이 클릭!!!!!!!");
        EventBus.Instance.Publish<UIEvents.SlotClicked>(new UIEvents.SlotClicked(item, this));
    }
}
