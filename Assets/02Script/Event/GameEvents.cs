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
        public PortalSwitchScene(SceneType nextScene, Vector3 targetPoint) { 
            this.nextScene = nextScene;
            this.targetPoint = targetPoint;
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


    // EnemyState 변경 알림
    public struct UpdateEnemy { }

    // Stamina 고갈
    public struct StaminaDepleted { }


    // 달리기 가능
    public struct StaminaRecovered { }


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

}
