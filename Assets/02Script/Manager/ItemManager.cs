
public enum ItemType
{ 
    Pickable,
    Interactable,
    Inspectable,
    Readable,
}


public class ItemManager : Singleton<ItemManager>
{
    // 상호작용시에 레이캐스트 쏴서 관리 (현재 상호작용중인 아이템)
    // 캐싱 필요할 시 static으로 둘 것


}
