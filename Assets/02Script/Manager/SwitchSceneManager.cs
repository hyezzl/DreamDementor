using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 전환 관리
/// </summary>
public class SwitchSceneManager : Singleton<SwitchSceneManager>
{
    public SceneType curScene;   // 현재씬
    public GameObject player;
    private Vector3 spawnPoint = Vector3.zero;
    private bool isPortal = false;      // 포탈이동에 의한 씬이동인지?

    public CharacterController ch;


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

        SceneManager.LoadScene(evt.nextScene.ToString());
        curScene = evt.nextScene; // 캐싱
    }

    private void OnPortal(GameEvents.PortalSwitchScene evt) { 
        // 이동하려는 씬과 현재 씬이 같을 때
        if(curScene == evt.nextScene){
            isPortal = true;
            spawnPoint = evt.targetPoint; 

            // 위치 변경
            MoveTransform();

            return;
        }

        Debug.Log($"{evt.nextScene}씬으로 이동~!");
        Debug.Log($"소환 위치 : {evt.nextScene.ToString()} - {evt.targetPoint}");
        isPortal = true;
        spawnPoint = evt.targetPoint;       // 소환위치 저장

        SceneManager.LoadScene(evt.nextScene.ToString());
        curScene = evt.nextScene; // 캐싱
    }

    // 씬 로드 시 플레이어 위치 지정
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = spawnPoint;
            }
            isPortal = false;
        }
    }

    // 같은 씬에서 이동 시 위치만 이동
    private void MoveTransform(){
        if(isPortal)
        {
            Debug.Log($"이동할 좌표!!! : {spawnPoint}");
            //GameObject player = GameObject.FindGameObjectWithTag("Player");

            if(player != null){
                ch.enabled = false;
                player.transform.position = spawnPoint;
                //player.transform.position += Vector3.one;
                //player.SetActive(false);
                Debug.Log($"{player.transform.position}");
                ch.enabled = true;
            }else{
                Debug.Log("플레이어가 널");
            }
            isPortal = false;
        }
    }
}
