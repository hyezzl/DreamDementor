using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    // 게임모드 변경
    public struct GameModeChange
    {
        public GameMode mode; // 새로 바뀔 모드

        public GameModeChange(GameMode mode)
        {
            Debug.Log($" √ {mode} 활성화 √");
            this.mode = mode;
        }
    }

    // 이벤트 발생
    public struct PlayEvent
    {
        public string eventID;

        public PlayEvent(string eventID)
        {
            this.eventID = eventID;
        }
    }

    // 선택지 포함된 이벤트 발생
    public struct PlayChoiceEvent
    {
        public string eventID;
        public PlayChoiceEvent(string eventID) { 
            this.eventID = eventID;
        }
    }

    public struct EndTimeline { }

    public struct EnemyStateChange
    {
        public EnemyState state;
        public EnemyStateChange(EnemyState state)
        {
            Debug.Log($" √ Enemy : {state} 활성화 √");
            this.state = state;
        }
    }

    // 카메라 교체
    public struct CameraShift
    {
        public CameraType type;
        public int blend;
        public CameraShift(CameraType type, int blend) {
            this.type = type;
            this.blend = blend;
        }
    }


    // 시간멈춤 이벤트
    public struct StopTime
    {
        public bool audioStop;
        public StopTime(bool audioStop) {
            this.audioStop = audioStop;
        }
    }

    // 시간 재개
    public struct FlowTime
    {
        public bool audioPlay;
        public FlowTime(bool audioPlay) {
            this.audioPlay = audioPlay;
        }
    }


    // 게임오버
    public struct GameOver
    {
        public SceneType scene;
        public DeathType type;
        public GameOver(SceneType scene, DeathType type) {
            this.scene = scene;
            this.type = type;
        }
    }

    // (강제) 씬이동 이벤트
    public struct SwitchScene
    {
        public SceneType nextScene;
        public SwitchScene(SceneType nextScene) {
            this.nextScene = nextScene;
        }
    }

    // (포탈) 씬이동
    public struct PortalSwitchScene
    { 
        public SceneType nextScene;
        public Vector3 targetPoint;
        public Direction4 targetDir;
        public PortalSwitchScene(SceneType nextScene, Vector3 targetPoint, Direction4 targetDir) { 
            this.nextScene = nextScene;
            this.targetPoint = targetPoint;
            this.targetDir = targetDir;
        }
    }

    // 인칭 변경
    public struct AspectChange
    {
        public AspectMode mode;
        public AspectChange(AspectMode mode) {
            this.mode = mode;
        }
    }


    // 아이템 습득
    public struct GetItem
    {
        public IActionItem item;
        public GetItem(IActionItem item) {
            this.item = item;
        }
    }

    // 강제 아이템 습득
    public struct PutItem
    {
        public ItemType type;
        public int itemID;
        public PutItem(ItemType type, int itemID) {
            this.type = type;
            this.itemID = itemID;
        }
    }

    // 강제 아이템 삭제
    public struct UseItem
    {
        public int itemID;
        public UseItem(int itemID) { 
            this.itemID = itemID;
        }
    }


    // EnemyState 변경 알림
    public struct UpdateEnemy { }

    // 이벤트 종료 (저장)
    public struct EndEvent
    {
        public string eventID;

        public EndEvent(string eventID)
        {
            this.eventID = eventID;
            Debug.Log($"{eventID} 이벤트 종료 후 저장!");
        }
    }


    // 이벤트 존 비활성화
    public struct ActiveZone
    {
        public string zoneID;
        public bool isActive;
        public ActiveZone(string zoneID, bool isActive) { 
            this.zoneID = zoneID;
            this.isActive = isActive;
        }
    }

    // 카메라 필터 활성화
    public struct FilterOn
    {
        public FilterType type;
        //public ICameraFilter filter;
        public bool isLasting;
        public float duration;
        public FilterOn(FilterType type, bool isLasting = true, float duration = 1f) {
            this.type = type;
            this.isLasting = isLasting;
            this.duration = isLasting ? 0f : duration;
        }
    }

    // 카메라 필터 비활성화
    public struct FilterOff
    {
        //public FilterType type;
        //public FilterOff(FilterType type) {
        //    this.type = type;
        //}
    }

    // 브금 재생
    public struct PlayBGM
    {
        public BGMType type;
        public PlayBGM(BGMType type) {
            this.type = type;
        }
    }


    // 브금 중단
    public struct StopBGM { }


    // 플레이어 방향 / 애니메이션 강제
    public struct ForceDir
    {
        public Direction4 dir;
        public ForceDir(Direction4 dir) {
            this.dir = dir;
        }
    }


    ///  스태미너 관련 이벤트들  ///////////////
    // 1 . Stamina -> PlayerMove

    // 스태미너가 변할때 (UI업데이트)
    public struct OnStaminaChanged 
    {
        public float curStamina;
        public float maxStamina;
        public OnStaminaChanged(float curStamina, float maxStamina) {
            this.curStamina = curStamina;
            this.maxStamina = maxStamina;
        }
    }


    //플레이어가 달리기 시작할때
    public struct OnSprintStart { }


    // 달리기를 멈추고 걷기 시작
    public struct OnSprintStop { }

    // 스태미나가 0이되어 더이상 달릴 수 없을때
    public struct OnStaminaDepleted { }


    //스태미너가 전부 회복되었을때
    public struct OnStaminaRecovered { }

    // 스태미너 충전이 시작
    public struct OnStaminaRecoverStart { }



    // 2 . PlayerMove -> Stamina (동기화)
    // 달리기 가능 여부 변경
    public struct ChangeSprintState
    {
        public bool canRunning;
        public ChangeSprintState(bool canRunning) { 
            this.canRunning = canRunning;
        }
    }

    //////////////////////////////////////////



    ///  체력(정신력) 관련 이벤트들  ///////////////

    // 정신력이 변할 때 (업데이트)
    public struct OnHpChange
    {
        //public SceneType curScene;
        public Stage stage;
        public int preHp;
        public int newHp;
        public OnHpChange(Stage stage, int preHp, int newHp)
        //public OnHpChange(SceneType curScene, int preHp, int newHp) 
        //public OnHpChange(int preHp, int newHp)
        {
            //this.curScene = curScene;
            this.stage = stage;
            this.preHp = preHp;
            this.newHp = newHp;
        }
    }

    // 피해를 입었을 때
    public struct OnDamaged
    {
        //public SceneType curScene;
        public int damage;      // 데미지량
        //public OnDamaged(SceneType curScene, int damage) {
        public OnDamaged(int damage) { 
            //this.curScene = curScene;
            this.damage = damage;
        }
    }

    // HP(정신력)가 0이 되었을 때
    public struct OnHpDepeleted
    {
        public SceneType curScene;
        public OnHpDepeleted(SceneType curScene) { 
            this.curScene = curScene;
        }
    }

    // 씬 변경에 의한 HP 초기화 시점
    public struct OnHpInit
    { 
        public SceneType curScene;
        public OnHpInit(SceneType curScene) { 
            this.curScene = curScene;
        }
    }


    public struct OnHeal
    { 
        //public SceneType curScene;
        public int heal;
        //public OnHeal(SceneType curScene, int heal) {
        public OnHeal(int heal) {
            //this.curScene = curScene;
            this.heal = heal;
        }
    }


    //////////////////////////////////////////

}
