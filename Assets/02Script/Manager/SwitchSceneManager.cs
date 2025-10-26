using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("Effect Setting")]
    [SerializeField] private float fadeDuration = 2.5f;
    [SerializeField] private float fadeInVal = 1f;
    [SerializeField] private float fadeOutVal = -0.15f;

    private CameraFilterPack_FX_Spot spot;
    private PlayerController pc;

    protected override void DoAwake()
    {
        base.DoAwake();
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("SwitchSceneManager - Failed to Load PlayerController");

        // 메인 카메라에서 필터 컴포넌트 받아오기
        Camera mainCam = Camera.main;
        if (mainCam != null)
            spot = mainCam.GetComponent<CameraFilterPack_FX_Spot>();
        if (spot == null)
            Debug.LogError("SwitchSceneManager - CameraFilterPack_FX_Spot 없음!");
        else
            // 초기값
            spot.Radius = fadeInVal;


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

        StartCoroutine(CloseScene(evt.nextScene));
    }

    private void OnPortal(GameEvents.PortalSwitchScene evt) { 
        // 이동하려는 씬과 현재 씬이 같을 때
        if(curScene == evt.nextScene){
            isPortal = true;
            spawnPoint = evt.targetPoint; 
            return;
        }

        Debug.Log($"{evt.nextScene}씬으로 이동~!");
        Debug.Log($"소환 위치 : {evt.nextScene.ToString()} - {evt.targetPoint}");
        isPortal = true;
        spawnPoint = evt.targetPoint;       // 소환위치 저장

        StartCoroutine(CloseScene(evt.nextScene));
    }

    // 씬 로드 시
    // + 플레이어 위치 지정
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //StartCoroutine(FadeInScene());

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


    // 전환 효과 후 씬이동
    private IEnumerator CloseScene(SceneType nextScene) {
        yield return StartCoroutine(FadeOutScene());

        pc.CurScene = nextScene;
        curScene = nextScene;

        SceneManager.LoadScene(nextScene.ToString());
    }



    //////////////// 씬 전환 효과
    // 씬 닫힐 때 효과
    private IEnumerator FadeOutScene()
    {
        if (spot == null) yield break;

        float elapsed = 0f;
        spot.Radius = fadeInVal;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / fadeDuration);
            spot.Radius = Mathf.Lerp(fadeInVal, fadeOutVal, t);

            yield return null;
        }
        spot.Radius = fadeOutVal;
    }


    // 씬 열릴 때 효과
    private IEnumerator FadeInScene()
    {
        if (spot == null) yield break;
        Debug.Log("씬들어오면 여기가 실행되어야함!!!");

        float elapsed = 0f;
        spot.Radius = fadeOutVal;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / fadeDuration);
            spot.Radius = Mathf.Lerp(fadeOutVal, fadeInVal, t);

            yield return null;
        }
        spot.Radius = fadeInVal;
    }
}
