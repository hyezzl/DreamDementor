using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 전환 관리
/// </summary>
[DefaultExecutionOrder(-10)]
public class SwitchSceneManager : Singleton<SwitchSceneManager>
{
    private SceneType curScene = SceneType.TitleScene;
    public SceneType CurScene
    {
        get => curScene;
        set => curScene = value;
    }

    private GameObject player;
    private Vector3 spawnPoint = Vector3.zero;
    private Direction4 spawnDir;
    private bool isPortal = false;      // 포탈이동에 의한 씬이동인지?

    [Header("Effect Setting")]
    [SerializeField] private float fadeDuration = 2.5f;
    [SerializeField] private float fadeInVal = 1f;
    [SerializeField] private float fadeOutVal = -0.15f;

    private CameraFilterPack_FX_Spot spot;



    protected override void DoAwake()
    {
        base.DoAwake();

        // 메인 카메라에서 필터 컴포넌트 받아오기
        Camera mainCam = Camera.main;
        if (mainCam != null)
            spot = mainCam.GetComponent<CameraFilterPack_FX_Spot>();
        if (spot == null)
            Debug.Log("SwitchSceneManager - CameraFilterPack_FX_Spot 없음!");
        else
            // 초기값
            spot.Radius = fadeInVal;
    }


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.SwitchScene>(SwitchScene);
        EventBus.Instance.Subscribe<GameEvents.PortalSwitchScene>(OnPortal);
        EventBus.Instance.Subscribe<GameEvents.ReloadScene>(OnReload);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.SwitchScene>(SwitchScene);
        EventBus.Instance.Unsubscribe<GameEvents.PortalSwitchScene>(OnPortal);
        EventBus.Instance.Unsubscribe<GameEvents.ReloadScene>(OnReload);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void SwitchScene(GameEvents.SwitchScene evt) {
        Debug.Log($"{evt.nextScene}씬으로 이동~!");
        isPortal = false;

        StartCoroutine(CloseScene(evt.nextScene));
    }

    private void OnPortal(GameEvents.PortalSwitchScene evt) {
        // 이동하려는 씬과 현재 씬이 같을 때
        if (curScene == evt.nextScene) {
            Debug.Log($"{curScene} : 이동하려는 씬과 현재씬이 같음!");
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

    // 씬 리로드
    private void OnReload(GameEvents.ReloadScene evt) {
        SceneManager.LoadScene(curScene.ToString());
    }


    // 씬 로드 시
    // + 플레이어 위치 지정
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSpotComponent();    // 씬 로드 될때마다 SpotComponent 새로 찾아옴

        string sceneName = scene.name;
        if (System.Enum.TryParse(sceneName, out SceneType sceneType))
        {
            // 변환 성공 시 씬의 시작 이벤트 발행
            EventBus.Instance.Publish<GameEvents.SceneStart>(new GameEvents.SceneStart(sceneType));
        }
        else { Debug.Log($"씬 {sceneName} -> SceneType 변환 실패 "); }

        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene() {
        yield return null;
        yield return null;

        if (isPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            CharacterController cc = player?.GetComponent<CharacterController>();

            if (player != null)
            {
                if (cc != null) cc.enabled = false;

                player.transform.position = spawnPoint;

                // 플레이어 방향 강제 설정
                EventBus.Instance.Publish(new GameEvents.ForceDir(spawnDir));

                if (cc != null) cc.enabled = true;
            }
            isPortal = false;
        }
    }


    // 전환 효과 후 씬이동
    private IEnumerator CloseScene(SceneType nextScene) {
        // 씬 닫기 직전 퀘스트UI 강제 닫기
        QuestUI questUI = FindObjectOfType<QuestUI>();
        if (questUI != null)
            questUI.CloseQuest();         // 알파값 0으로

        if (curScene != SceneType.TutorialScene && curScene != SceneType.TitleScene) 
        {
            yield return StartCoroutine(FadeOutScene());
            Debug.Log($"{curScene} > {nextScene} 으로의 씬 페이드아웃 실행! (완료시점)");
        }

        yield return null;

        curScene = nextScene;

        SceneManager.LoadScene(nextScene.ToString());
    }



    //////////////// 씬 전환 효과
    // 씬 닫힐 때 효과
    private IEnumerator FadeOutScene()
    {
        if (spot == null) {
            Debug.Log("Spot이 널이므로 생략");   
            yield break;
        }
        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));


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


        yield return null;
    }

    public Vector3 GetSpawnPoint()
    {
        return spawnPoint;
    }

    public Direction4 GetSpawnDir()
    {
        return spawnDir;
    }

    // 매씬 Spot 새로 찾기
    private void FindSpotComponent() {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            spot = mainCam.GetComponent<CameraFilterPack_FX_Spot>();
            if (spot != null)
            {
                spot.Radius = fadeInVal;
            }
            else
            {
                Debug.LogWarning("CameraFilterPack_FX_Spot 컴포넌트를 찾을 수 없음!");
            }
        }
        else
        {
            Debug.LogWarning("Main Camera를 찾을 수 없음!");
        }
    }

}
