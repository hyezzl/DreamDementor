using UnityEngine;

public class ItemUIDGenerator : MonoBehaviour
{
    private static int curUID = 0;

    public static int GenerateUID() { 
        return curUID++;
    }

    // 게임종료 이후 다시 불러오기
    public static void SetStartUID(int loadUID) { 
        curUID = loadUID;
    }
}
