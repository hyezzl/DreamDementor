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

    // 선택지 선택
    public struct MakeChoice
    {
        public string choiceID;
        public int selectIdx;
        public MakeChoice(string choiceID, int selectIdx) { 
            this.choiceID = choiceID;
            this.selectIdx = selectIdx;
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
}
