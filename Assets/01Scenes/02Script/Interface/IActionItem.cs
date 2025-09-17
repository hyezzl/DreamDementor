
public interface IActionItem
{
    int GetItemID();

    void Init(IDatabase db); // 공통 초기화 함수

    void Interact();

    ItemType GetItemType();
}