using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public Dictionary<Stage, int> curHp = new();
    public List<ItemInstance> inventory = new();
    public List<string> completedEvents = new();                // 실행된 이벤트 목록
    public Dictionary<SceneType, bool> visitScene = new();     // 씬 방문기록
    public Dictionary<string, bool> zoneActiveMap = new();             // 존 활성화/비활성화 관리
}



public class SaveManager : Singleton<SaveManager>
{
    private const string PlayerHPKey = "PlayerHP";
    private const string InventoryKey = "Inventory";
    private const string CompletedEventsKey = "CompletedEvents";
    private const string VisitSceneKey = "VisitScene";
    private const string ZoneActiveMapKey = "ZoneActiveMap";

    // 현재 저장데이터
    public SaveData curSaveData = new();



    // 저장함수 호출
    public void SaveGame() {
        // 플레이어 체력 저장
        string hpJson = JsonUtility.ToJson(new SerializableHp(curSaveData.curHp));
        PlayerPrefs.SetString(PlayerHPKey, hpJson);

        // 인벤토리 저장 - ItemInstance 리스트를 int[]로 변환 후 JSON 직렬화
        List<int> itemIDs = new List<int>();
        foreach (var item in curSaveData.inventory)
        {
            itemIDs.Add(item.itemID);
        }
        string inventoryJson = JsonUtility.ToJson(new SerializableIntList(itemIDs));
        PlayerPrefs.SetString(InventoryKey, inventoryJson);

        // 완료 이벤트 리스트는 JSON 문자열로 저장
        string eventsJson = JsonUtility.ToJson(new SerializableStringList(curSaveData.completedEvents));
        PlayerPrefs.SetString(CompletedEventsKey, eventsJson);

        // 씬 방문 기록은 키/값 쌍을 문자열 형식으로 저장 (간단하게 JSON 배열로)
        var visitSceneList = new List<BoolStringPair>();
        foreach (var kvp in curSaveData.visitScene)
        {
            visitSceneList.Add(new BoolStringPair(kvp.Key.ToString(), kvp.Value));
        }
        string visitSceneJson = JsonUtility.ToJson(new SerializableBoolStringList(visitSceneList));
        PlayerPrefs.SetString(VisitSceneKey, visitSceneJson);

        // 존 활성화 지도 저장 (같은 방식을 사용)
        var zoneList = new List<BoolStringPair>();
        foreach (var kvp in curSaveData.zoneActiveMap)
        {
            zoneList.Add(new BoolStringPair(kvp.Key, kvp.Value));
        }
        string zoneJson = JsonUtility.ToJson(new SerializableBoolStringList(zoneList));
        PlayerPrefs.SetString(ZoneActiveMapKey, zoneJson);

        PlayerPrefs.Save();

        Debug.Log("게임 저장 완료!");
    }

    // 불러오기 함수 호출 (게임 시작 혹은 씬 시작시 호출)
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey(PlayerHPKey))
        {
            string hpJson = PlayerPrefs.GetString(PlayerHPKey);
            SerializableHp loadedHp = JsonUtility.FromJson<SerializableHp>(hpJson);
            if (loadedHp != null && loadedHp.hpList != null)
                curSaveData.curHp = loadedHp.ToDictionary();
            else
                curSaveData.curHp = new Dictionary<Stage, int>(); // 예외 시 기본값
        }

        if (PlayerPrefs.HasKey(InventoryKey))
        {
            string inventoryJson = PlayerPrefs.GetString(InventoryKey);
            SerializableIntList sList = JsonUtility.FromJson<SerializableIntList>(inventoryJson);
            curSaveData.inventory.Clear();
            foreach (var id in sList.list)
            {
                curSaveData.inventory.Add(new ItemInstance(id, ""));
            }
        }

        if (PlayerPrefs.HasKey(CompletedEventsKey))
        {
            string eventsJson = PlayerPrefs.GetString(CompletedEventsKey);
            var sEvents = JsonUtility.FromJson<SerializableStringList>(eventsJson);
            curSaveData.completedEvents = new List<string>(sEvents.list);
        }

        if (PlayerPrefs.HasKey(VisitSceneKey))
        {
            string visitJson = PlayerPrefs.GetString(VisitSceneKey);
            var sVisitList = JsonUtility.FromJson<SerializableBoolStringList>(visitJson);
            curSaveData.visitScene.Clear();
            foreach (var pair in sVisitList.list)
            {
                Enum.TryParse(pair.key, out SceneType scene);
                curSaveData.visitScene[scene] = pair.value;
            }
        }

        if (PlayerPrefs.HasKey(ZoneActiveMapKey))
        {
            string zoneJson = PlayerPrefs.GetString(ZoneActiveMapKey);
            var sZoneList = JsonUtility.FromJson<SerializableBoolStringList>(zoneJson);
            curSaveData.zoneActiveMap.Clear();
            foreach (var pair in sZoneList.list)
            {
                curSaveData.zoneActiveMap[pair.key] = pair.value;
            }
        }

        Debug.Log("게임 불러오기 완료!");
        ApplyLoadedData();
    }


    // 불러온 데이터를 게임에 반영하는 함수
    public void ApplyLoadedData()
    {
        // 정신력 반영
        foreach (Stage stage in Enum.GetValues(typeof(Stage)))
        {
            if (curSaveData.curHp.ContainsKey(stage))
            {
                PlayerController.Instance.curHp[stage] = curSaveData.curHp[stage];
            }
            else
            {
                PlayerController.Instance.curHp[stage] = 100; // 기본 체력
            }
        }

        // 인벤토리 반영
        InventoryManager.Instance.LoadInventoryFromSave(curSaveData.inventory);

        // 완료이벤트 목록
        EventHistoryManager.Instance.LoadCompletedEvents(curSaveData.completedEvents);

        // 씬 방문 기록
        EventHistoryManager.Instance.LoadVisitScenes(curSaveData.visitScene);

        // 존 활성화 로그
        ZoneManager.zoneActiveMap = new Dictionary<string, bool>(curSaveData.zoneActiveMap);

        Debug.Log("저장 데이터 게임 내 반영 완료");
    }


    // 보조 클래스들
    [Serializable]
    public struct StageHpData
    {
        public Stage stage;  // 저장할 Stage enum 키
        public int hp;       // 해당 Stage의 체력 값
    }

    [Serializable]
    public class SerializableHp
    {
        public List<StageHpData> hpList = new();
        public SerializableHp(Dictionary<Stage, int> hpDict)
        {
            foreach (var pair in hpDict)
            {
                hpList.Add(new StageHpData { stage = pair.Key, hp = pair.Value });
            }
        }
        public Dictionary<Stage, int> ToDictionary()
        {
            Dictionary<Stage, int> dict = new();
            foreach (var data in hpList)
            {
                dict[data.stage] = data.hp;
            }
            return dict;
        }
    }
        [Serializable]
    private class SerializableIntList
    {
        public List<int> list;
        public SerializableIntList(List<int> list) { this.list = list; }
    }

    [Serializable]
    public class SerializableStringList
    {
        public List<string> list;
        public SerializableStringList(List<string> list) { this.list = list; }
    }

    [Serializable]
    public class SerializableBoolStringList
    {
        public List<BoolStringPair> list;
        public SerializableBoolStringList(List<BoolStringPair> list) { this.list = list; }
    }

    [Serializable]
    public class BoolStringPair
    {
        public string key;
        public bool value;
        public BoolStringPair(string key, bool value) { this.key = key; this.value = value; }
    }


}
