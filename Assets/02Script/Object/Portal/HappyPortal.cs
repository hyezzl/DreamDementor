using UnityEngine;

public class HappyPortal : Portal
{
    protected override void CustomSave()
    {
        SaveManager.Instance.AutoSave();    // npc, zone 정보 저장
    }
}
