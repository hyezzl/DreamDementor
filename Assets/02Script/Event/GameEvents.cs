using Cinemachine;
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
            this.state = state;
        }
    }

    // 카메라 교체
    public struct CameraShift
    {
        public CameraType type;
        public CameraShift(CameraType type) { 
            this.type = type;
        }
    }
}
