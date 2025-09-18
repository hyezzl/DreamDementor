using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Refs")]
    [SerializeField] private Image icon;
    [SerializeField] private GameObject slot;
    [SerializeField] private Sprite nothing;

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

            switch (itemInst.type)
            {
                case ItemType.Pickable:
                    if (database == null) Debug.Log("database가 널!");
                    var dataP = database.GetPickable(itemInst.itemID);
                    icon.sprite = dataP.icon;
                    if (dataP.icon == null)
                        Debug.Log($"{itemInst.itemID}의 아이콘 불러오지 못함!");
                    break;

                case ItemType.Eatable:
                    if (database == null) Debug.Log("database가 널!");
                    var dataE = database.GetEatable(itemInst.itemID);
                    icon.sprite = dataE.icon;
                    if (dataE.icon == null)
                        Debug.Log($"{itemInst.itemID}의 아이콘 불러오지 못함!");
                    break;
            }
        }
        else {
            icon.enabled = false;
            icon.sprite = nothing;
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
