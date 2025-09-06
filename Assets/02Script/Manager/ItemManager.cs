
public enum ItemType
{ 
    Pickable,
    Eatable,
    Interactable,
    Inspectable,
    //Readable,
}


public class ItemManager : Singleton<ItemManager>
{
    // 상호작용시에 레이캐스트 쏴서 관리 (현재 상호작용중인 아이템)
    // 캐싱 필요할 시 static으로 둘 것

    // 3인칭 일때, 레이캐스트

    // 1인칭 일 때, 시야 콜라이더가 보고있는 값 static 관리


}
