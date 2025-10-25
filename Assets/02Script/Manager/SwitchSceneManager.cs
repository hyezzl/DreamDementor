using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// 씬 전환 관리
/// </summary>
public class SwitchSceneManager : Singleton<SwitchSceneManager>
{
    public SceneType curScene;   // 현재씬
    public GameObject player;
    private Vector3 spawnPoint = Vector3.zero;
    private Direction4 spawnDir;
    private bool isPortal = false;      // 포탈이동에 의한 씬이동인지?

    public CharacterController ch;
    private PlayerController pc;

    protected override void DoAwake()
    {
        base.DoAwake();
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("SwitchSceneManager - Failed to Load PlayerController");
    }
    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.SwitchScene>(SwitchScene);
        EventBus.Instance.Subscribe<GameEvents.PortalSwitchScene>(OnPortal);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.SwitchScene>(SwitchScene);
        EventBus.Instance.Unsubscribe<GameEvents.PortalSwitchScene>(OnPortal);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void SwitchScene(GameEvents.SwitchScene evt) {
        Debug.Log($"{evt.nextScene}씬으로 이동~!");
        isPortal = false;

        // 씬 캐싱 변경
        pc.CurScene = evt.nextScene;
        curScene = evt.nextScene; // 캐싱

        SceneManager.LoadScene(evt.nextScene.ToString());
    }

    private void OnPortal(GameEvents.PortalSwitchScene evt) { 
        // 이동하려는 씬과 현재 씬이 같을 때
        if(curScene == evt.nextScene){
            isPortal = true;
            spawnPoint = evt.targetPoint; 

            // 위치 변경
            //MoveTransform();

            return;
        }

        // 씬 캐싱 변경
        pc.CurScene = evt.nextScene;
        curScene = evt.nextScene; // 캐싱

        Debug.Log($"{evt.nextScene}씬으로 이동~!");
        Debug.Log($"소환 위치 : {evt.nextScene.ToString()} - {evt.targetPoint}");
        isPortal = true;
        spawnPoint = evt.targetPoint;       // 소환위치 저장

        SceneManager.LoadScene(evt.nextScene.ToString());
    }

    // 씬 로드 시
    // + 플레이어 위치 지정
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        if (System.Enum.TryParse(sceneName, out SceneType sceneType))
        {
            // 변환 성공 시 씬의 시작 이벤트 발행
            EventBus.Instance.Publish<GameEvents.SceneStart>(new GameEvents.SceneStart(sceneType));
        }
        else { Debug.Log($"씬 {sceneName} -> SceneType 변환 실패 "); }


        if (isPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = spawnPoint;

                // 플레이어 방향 강제 설정
                EventBus.Instance.Publish(new GameEvents.ForceDir(spawnDir));
            }
            isPortal = false;
        }
    }
}
