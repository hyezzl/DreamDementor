using UnityEngine;

/// <summary>
/// 얻은 아이템
/// </summary>
public class ItemInstance
{
    public int itemID;
    public int uniqueID; // 개별 인스턴스 구별 ID
    public string pairID;
    public ItemType type;

    public ItemInstance(int itemID, string pairID) { 
        this.itemID = itemID;
        uniqueID = ItemUIDGenerator.GenerateUID();
        this.pairID = pairID;

        // Type분기
        char first = itemID.ToString()[0];
        switch (first) {
            case '1':
                type = ItemType.Pickable;
                break;
            case '2':
                type = ItemType.Eatable;
                break;
        }
    }
}
