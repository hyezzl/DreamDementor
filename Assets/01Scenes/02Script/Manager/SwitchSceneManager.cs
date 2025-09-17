using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 전환 관리
/// </summary>
public class SwitchSceneManager : Singleton<SwitchSceneManager>
{
    private SceneType curScene;   // 현재씬

    // 임시
    private Vector3 happyStartPos = new Vector3(0.5f, 0, 0);


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.SwitchScene>(SwitchScene);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.SwitchScene>(SwitchScene);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void SwitchScene(GameEvents.SwitchScene evt) {
        Debug.Log($"{evt.nextScene}씬으로 이동~!");

        SceneManager.LoadScene(evt.nextScene.ToString());
        curScene = evt.nextScene; // 캐싱
    }

    // 씬 로드 시 플레이어 위치 지정
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        //if (curScene == SceneType.HappyScene) { 
        //    player.transform.position = happyStartPos;
        //    playerAnim.Rebind();
        //    playerAnim.SetFloat("lookX", 0f);
        //    playerAnim.SetFloat("lookY", -1f);

        //    PlayerMove pm = player.GetComponent<PlayerMove>();
        //    pm.PreDir = Vector3.back;
        //}
    }

}
