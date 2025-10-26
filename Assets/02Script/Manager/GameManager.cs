using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 게임 내 시간 관리
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public static bool IsPaused = false;

    protected override void DoAwake()
    {
        base.DoAwake();
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;

        if (IsPaused)
        {
            Time.timeScale = 0f;  // 게임 내 시간 멈춤
            AudioListener.pause = true; // 오디오 멈춤
        }
        else
        {
            Time.timeScale = 1f;  // 시간 다시 흐르게
            AudioListener.pause = false; // 오디오 다시 재생
        }
    }

    /// 고치기
    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.GameModeChange>(OnModeChange);
        EventBus.Instance.Subscribe<GameEvents.StopTime>(OnStop);
        EventBus.Instance.Subscribe<GameEvents.FlowTime>(OnFlow);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GameModeChange>(OnModeChange);
        EventBus.Instance.Unsubscribe<GameEvents.StopTime>(OnStop);
        EventBus.Instance.Unsubscribe<GameEvents.FlowTime>(OnFlow);
    }

    private void OnModeChange(GameEvents.GameModeChange evt)
    {
        //if (evt.mode == GameMode.PauseMode)
        //{
        //    StopTime();
        //}
        //else
        //    FlowTime();

        switch (evt.mode) {
            case GameMode.InspectMode:
                // 커서 가림
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.None;
                break;

            case GameMode.PauseMode:
            case GameMode.UIPuzzleMode:
                // 커서 생김
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;
        }
    }

    private void OnStop(GameEvents.StopTime evt) {
        if (evt.audioStop)
        {
            StopTime();
        }
        else
            HalfStop();
    }

    private void OnFlow(GameEvents.FlowTime evt) {
        if (evt.audioPlay) {
            FlowTime();
            AudioListener.pause = false;
        }
        else
            FlowTime();
    }


    private void StopTime()
    {
        Time.timeScale = 0;
        Debug.Log("시간 멈춤!!!");
        AudioListener.pause = true;
    }

    private void HalfStop()
    { 
        Time.timeScale = 0;
        Debug.Log("시간 멈춤!!!");
    }

    private void FlowTime()
    {
        Debug.Log("시간 재개!!");
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}
