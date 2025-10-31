using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameEvents;


[Serializable]
public class SaveData
{
    //public Dictionary<Stage, int> curHp = new();
    public int curHP;
    public SceneType scene;                                        // 현재씬
    public List<ItemInstance> inventory = new();
    public List<string> completedEvents = new();                // 실행된 이벤트 목록
    public Dictionary<SceneType, bool> visitScene = new();     // 씬 방문기록
    public Dictionary<string, bool> zoneActiveMap = new();             // 존 활성화/비활성화 관리
}



public class SaveManager : Singleton<SaveManager>
{
    private const string SaveKey = "GameSave";

    public SaveData curSaveData = new SaveData();


    // 씬 필수 자동저장 함수 (가려는씬이 아니라 현재 위치한 씬)
    HashSet<SceneType> scenesToAutoSave = new HashSet<SceneType>
    {
        SceneType.HappyScene,
        //SceneType.SorrowScene,
    };

    // 씬 저장 함수 (가려는씬이 아니라 현재 위치한 씬)
    HashSet<SceneType> scenesToSave = new HashSet<SceneType>
    {
        SceneType.HappyScene,
        SceneType.HappyOneScene,
        //SceneType.SorrowScene,
    };


    // 로드될 특정 씬
    HashSet<SceneType> scenesToLoad = new HashSet<SceneType>
    {
        SceneType.HappyScene,
        SceneType.SorrowScene,
    };


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.SwitchScene>(OnSwitchScene01);
        EventBus.Instance.Subscribe<GameEvents.PortalSwitchScene>(OnSwitchScene02);

        EventBus.Instance.Subscribe<GameEvents.SceneStart>(OnSceneStart);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.SwitchScene>(OnSwitchScene01);
        EventBus.Instance.Unsubscribe<GameEvents.PortalSwitchScene>(OnSwitchScene02);

        EventBus.Instance.Unsubscribe<GameEvents.SceneStart>(OnSceneStart);
    }

    // 씬이동1
    private void OnSwitchScene01(GameEvents.SwitchScene evt) {
        if (scenesToAutoSave.Contains(evt.curScene))
        {
            Debug.Log($"&************오토세이브");
            AutoSave();
        }
        ManualSave();
        SaveGame();
    }

    // 씬이동2
    private void OnSwitchScene02(GameEvents.PortalSwitchScene evt) {
        if (scenesToAutoSave.Contains(evt.curScene))
        {
            Debug.Log($"&************오토세이브");
            AutoSave();
        }
        ManualSave();
        SaveGame();
    }

    // 씬로드
    private void OnSceneStart(GameEvents.SceneStart evt) {
        // 필요시 씬 시작할 때 저장

        if (scenesToSave.Contains(evt.curScene))
        {
            ManualSave();
            SaveGame();
            // 오토는 생각좀해보고
        }
        else
        {
            Debug.Log($"{evt.curScene} 씬에서는 시작 시 저장 하지않음.");
        }
    }


    // 저장 함수
    public void SaveGame()
    {
        ES3.Save(SaveKey, curSaveData);
        Debug.Log("게임 데이터를 저장!");
    }

    // 자동 저장 (NPC + Zone)
    public void AutoSave() {
        // 모든 체크 오브젝트의 상태를 즉시 저장 
        ES3AutoSaveMgr.Current.Save();
    }

    // 불러오기 함수
    public bool LoadGame()
    {
        if (ES3.KeyExists(SaveKey))
        {
            curSaveData = ES3.Load<SaveData>(SaveKey);
            Debug.Log("데이터 로드!");
            return true;
        }
        else
        {
            Debug.Log("저장된 데이터가 없습니다");
            return false;
        }
    }

    // 자동 로드 (NPC + Zone)
    public void AutoLoad() {
        ES3AutoSaveMgr.Current.Load();
    }

    // 수동 저장
    private void ManualSave() {
        // 1. 현재 HP기록
        curSaveData.curHP = PlayerController.Instance.CurHP;
        //Stage curStage = PlayerController.Instance.Scene2Stage(SwitchSceneManager.Instance.CurScene);

        // 기존 curHP딕셔너리에 해당 스테이지 HP갱신
        //if (curSaveData.curHp == null) {
        //    curSaveData.curHp = new Dictionary<Stage, int>();
        //}

        //int playerHP = 100;     // 임시 기본값
        //if (PlayerController.Instance.curHp != null && PlayerController.Instance.curHp.ContainsKey(curStage))
        //{
        //    playerHP = PlayerController.Instance.curHp[curStage];
        //}
        //curSaveData.curHp[curStage] = playerHP;



        // 2. 현재 씬 기록
        curSaveData.scene = SwitchSceneManager.Instance.CurScene;

        // 3. 인벤토리 저장
        curSaveData.inventory = InventoryManager.Instance.GetInventory().ToList();

        // 4. 완료된 이벤트 목록 저장
        curSaveData.completedEvents = EventHistoryManager.Instance.GetCompletedEventsList();

        // 5. 씬 방문 기록 저장
        curSaveData.visitScene = EventHistoryManager.Instance.GetVisitSceneDict();
    }

    // 수동 로드
    private void ManualLoad() {
        // 저장 데이터에서 현재 씬 기준 스테이지 HP 가져와서 PlayerController에 세팅

        // 1. HP 로드
        //Stage currentStage = PlayerController.Instance.Scene2Stage(curSaveData.scene);

        //if (curSaveData.curHp != null && curSaveData.curHp.ContainsKey(currentStage))
        //{
        //    if (PlayerController.Instance.curHp == null)
        //        PlayerController.Instance.curHp = new Dictionary<Stage, int>();

        //    PlayerController.Instance.curHp[currentStage] = curSaveData.curHp[currentStage];
        //}
        if (PlayerController.Instance != null)
            PlayerController.Instance.SetCurHP(curSaveData.curHP, curSaveData.scene); // SetCurHP는 직접 구현 필요: 해당 씬 기준으로 HP 적용


        // 2. 씬
        if (curSaveData.scene != null) { 
            SwitchSceneManager.Instance.CurScene = curSaveData.scene;
        }

        // 3. 인벤토리
        if (curSaveData.inventory != null && curSaveData.inventory.Count > 0) {
            InventoryManager.Instance.LoadInventoryFromSave(curSaveData.inventory);
        }

        // 4. 완료된 이벤트 목록
        if (curSaveData.completedEvents != null && curSaveData.completedEvents.Count > 0)
        {
            EventHistoryManager.Instance.LoadCompletedEvents(curSaveData.completedEvents);
        }

        // 5. 씬 방문 기록 복원
        if (curSaveData.visitScene != null && curSaveData.visitScene.Count > 0)
        {
            EventHistoryManager.Instance.LoadVisitScenes(curSaveData.visitScene);
        }
    }


    public void ReloadScene(SceneType curScene) {
        // 1. 씬 방문 기록을 첫 방문으로 초기화
        EventHistoryManager.Instance.ResetVisitSceneRecord(curScene);

        // 2. 저장파일 있으면 불러오기 
        if (LoadGame())
        {
            //Stage restartStage = PlayerController.Instance.Scene2Stage(curScene);
            //if (curSaveData.curHp.ContainsKey(restartStage))
            //    curSaveData.curHp[restartStage] = 100;

            // 3. HP, 인벤토리, 이벤트, 씬방문 등 모든 데이터가 저장된 값으로 복원됨
            ManualLoad();

            // 4. 오토세이브 오브젝트(존, NPC 등) 상태 복원
            //AutoLoad();

            // 5. 씬 재시작
            UnityEngine.SceneManagement.SceneManager.LoadScene(curScene.ToString());
        }
        else {
            Debug.Log("저장된 데이터 없음!");
        }
    }
}
