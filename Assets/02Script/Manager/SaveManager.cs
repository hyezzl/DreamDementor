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
    public Dictionary<Stage, int> wholeHP = new();
}



public class SaveManager : Singleton<SaveManager>
{
    private const string SaveKey = "GameSave";

    public SaveData curSaveData = new SaveData();


    // 씬 필수 자동저장 함수 (가려는씬이 아니라 현재 위치한 씬)
    HashSet<SceneType> scenesToAutoSave = new HashSet<SceneType>
    {
        SceneType.HappyScene,
        SceneType.SorrowScene,
    };

    // 씬 저장 함수 (가려는씬이 아니라 현재 위치한 씬)
    HashSet<SceneType> scenesToSave = new HashSet<SceneType>
    {
        //SceneType.HappyScene,
        //SceneType.HappyOneScene,
        //SceneType.SorrowScene,
        //SceneType.SorrowOneScene,
    };

    // 씬 열릴 때 자동으로 저장할 씬
    // (씬 내부(SceneStart)에서 따로 저장하지 않는 씬들)
    HashSet<SceneType> OpenSceneSave = new HashSet<SceneType>
    {
        //SceneType.TutorialScene
    };


    // 로드될 특정 씬
    HashSet<SceneType> scenesToAutoLoad = new HashSet<SceneType>
    {
        SceneType.HappyScene,
        SceneType.SorrowScene,
    };

    // 세이브매니저는 싱글톤이니까.. 굳이 이벤트발행으로 저장하지말고
    // 외부에서 저장하게 하는 방법은 어떨까..?

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
        if (scenesToSave.Contains(evt.curScene)) {
            Debug.Log($"플레이어에 관한 정보가 저장! 씬정보 : {evt.curScene}");
            ManualSave();
            SaveGame();
        }
    }

    // 씬이동2
    private void OnSwitchScene02(GameEvents.PortalSwitchScene evt) {
        if (scenesToAutoSave.Contains(evt.curScene))
        {
            Debug.Log($"&************오토세이브");
            AutoSave();
        }
        if (scenesToSave.Contains(evt.curScene))
        {
            Debug.Log($"플레이어에 관한 정보가 저장! 씬정보 : {evt.curScene}");
            ManualSave();
            SaveGame();
        }
    }

    // 씬로드
    private void OnSceneStart(GameEvents.SceneStart evt) {
        // 필요시 씬 시작할 때 (강제)저장 (오토세이브X)

        if (OpenSceneSave.Contains(evt.curScene))
        {
            ManualSave();
            SaveGame();
        }
        //else
        //{
        //    Debug.Log($"{evt.curScene} 씬에서는 시작 시 저장 하지않음.");
        //}
    }


    // 저장 함수
    public void SaveGame()
    {
        ES3.Save(SaveKey, curSaveData);
        Debug.Log("게임 데이터를 저장!");
    }

    // 자동 저장 (NPC)
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
            return true;
        }
        else
        {
            Debug.Log("저장된 데이터가 없습니다");
            return false;
        }
    }

    // 자동 로드 (NPC + Zone)
    public void AutoLoad() 
    {
        ES3AutoSaveMgr.Current.Load();
    }

    // 수동 저장
    public void ManualSave() {
        // 1. 현재 HP기록
        curSaveData.curHP = PlayerController.Instance.CurHP;

        // 2. 현재 씬 기록
        curSaveData.scene = SwitchSceneManager.Instance.CurScene;

        // 3. 인벤토리 저장
        curSaveData.inventory = InventoryManager.Instance.GetInventory().ToList();

        // 4. 완료된 이벤트 목록 저장
        curSaveData.completedEvents = EventHistoryManager.Instance.GetCompletedEventsList();

        // 5. 씬 방문 기록 저장
        curSaveData.visitScene = EventHistoryManager.Instance.GetVisitSceneDict();

        // 6. 전체 HP 정보 저장
        curSaveData.wholeHP = PlayerController.Instance.wholeHP;
    }

    // 수동 로드
    public void ManualLoad() {
        // 1. HP 로드
        if (PlayerController.Instance != null)
            PlayerController.Instance.SetCurHP(curSaveData.curHP);


        // 2. 씬
        if (curSaveData.scene != SceneType.None) { 
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

        // 6. 전체 HP 정보 로드
        if (curSaveData.wholeHP != null && curSaveData.wholeHP.Count > 0)
        {
            PlayerController.Instance.wholeHP = new Dictionary<Stage, int>(curSaveData.wholeHP);
        }
    }


    /// <summary>
    /// 데드씬에서 씬 재로드하는 함수
    /// </summary>
    public void ReloadScene(SceneType curScene) {
        // 1. 씬 방문 기록을 첫 방문으로 초기화
        //EventHistoryManager.Instance.ResetVisitSceneRecord(curScene);

        // 2. 저장파일 있으면 불러오기 
        if (LoadGame())
        {
            ManualLoad();

            // 오토로드
            if (scenesToAutoLoad.Contains(curScene)) { 
                AutoLoad();
            }

            // 씬 재시작
            UnityEngine.SceneManagement.SceneManager.LoadScene(curScene.ToString());
        }
        else {
            Debug.Log("저장된 데이터 없음!");
        }
    }

    /// <summary>
    /// 타이틀씬에서 로드게임 하는 함수
    /// </summary>
    public void LoadGameInTitle() 
    {
        // 1. 저장된 데이터가 있는지 확인
        if (LoadGame())
        {
            // 씬 정보 불러와서 씬이동
            SceneType savedScene = curSaveData.scene;

            ManualLoad();

            // 특정 씬에서만 오토세이브 상태 복원
            if (scenesToAutoLoad.Contains(savedScene))
            {
                AutoLoad();
            }

            // 씬 전환
            UnityEngine.SceneManagement.SceneManager.LoadScene(savedScene.ToString());
        }
        else
        {
            Debug.Log("저장된 데이터가 없어서 새 게임 시퀀스 시작 필요");
        }
    }
}
