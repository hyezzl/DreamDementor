using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum BGMType
{ 
    TitleBGM        = 0,
    HappyBGM        = 1,
    SorrowBGM       = 2,
    EnemyBGM        = 3,
}

public enum SFXType
{ 
    footPrint,                  // 발자국소리
    takeDamage,                 // 데미지입을 때 나는 소리
    enemyScream,                // 귀신 등장할 때 나는 소리
    trafficAccident,            // 서린이 부모님 사고 사운드
    pencilSound,
    rainSound,                    // 빗소리
    enemyLaugh,                 // 슬픔맵 1인칭 귀신등장
}

/// <summary>
/// 사운드 전체 관리 ( 브금/효과음 + 설정값에 따른 변화값 저장 )
/// </summary>

[DefaultExecutionOrder(-1)]
public class SoundManager : Singleton<SoundManager>
{
    [Header("AudioMixer")]
    public AudioMixer am;

    [Header("AudioSource Ref")]
    public AudioSource bgmSource;
    public AudioSource sfxSource01;
    public AudioSource sfxSource02;
    public AudioSource loopSfxSource;       // 루프용 SFX오디오 소스

    [Header("Clip List")]
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;

    [Header("Sound Setting")]
    public float fadeDuration = 1f;

    // 현재 사운드 상태값
    public bool isBGMPlaying = false;
    public bool isSFXPlaying = false;

    // onlySFX 변수
    public bool isOnlySfxPlaying = false;

    private Coroutine bgmCoroutine;


    /// 현재 사운드설정값 저장
    private const string MasterVolKey = "MasterVolPref";
    private const string BGMVolKey = "BGMVolPref";
    private const string SFXVolKey = "SFXVolPref";
    private const string MasterVolLastKey = "MasterVolLastPref";
    private const string BGMVolLastKey = "BGMVolLastPref";
    private const string SFXVolLastKey = "SFXVolLastPref";

    public float masterVol = 1f; // 마스터 볼륨 저장
    public float bgmVol = 1f;    // BGM 볼륨 저장
    public float sfxVol = 1f;    // SFX 볼륨 저장


    protected override void DoAwake()
    {
        base.DoAwake();
        // 초기화 시 볼륨 설정 로드
        LoadVolumeSettings();
    }

    // 믹서 스냅샷이 Awake의 SetFloat을 덮을 수 있어서, 믹서가 준비된 뒤에 적용
    private IEnumerator Start()
    {
        ApplyMixerVolumes();
        yield return null;
        ApplyMixerVolumes();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        EventBus.Instance.Subscribe<GameEvents.NewStageStart>(OnStartNewStage);
        EventBus.Instance.Subscribe<GameEvents.StageEnd>(OnEndStage);

        EventBus.Instance.Subscribe<GameEvents.PlayBGM>(OnPlayBGM);
        EventBus.Instance.Subscribe<GameEvents.StopBGM>(OnStopBGM);
        EventBus.Instance.Subscribe<GameEvents.PlaySFX>(OnPlaySFX);
        EventBus.Instance.Subscribe<GameEvents.PlayOnlySFX>(OnPlayOnlySFX);
        EventBus.Instance.Subscribe<GameEvents.PlayLoopSFX>(OnPlayLoopSFX);
        EventBus.Instance.Subscribe<GameEvents.StopSFX>(OnStopSFX);
        // SFX
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        EventBus.Instance.Unsubscribe<GameEvents.NewStageStart>(OnStartNewStage);
        EventBus.Instance.Unsubscribe<GameEvents.StageEnd>(OnEndStage);

        EventBus.Instance.Unsubscribe<GameEvents.PlayBGM>(OnPlayBGM);
        EventBus.Instance.Unsubscribe<GameEvents.StopBGM>(OnStopBGM);
        EventBus.Instance.Unsubscribe<GameEvents.PlayOnlySFX>(OnPlayOnlySFX);
        EventBus.Instance.Unsubscribe<GameEvents.PlayLoopSFX>(OnPlayLoopSFX);
        EventBus.Instance.Unsubscribe<GameEvents.StopSFX>(OnStopSFX);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyMixerVolumes();
    }

    // 새로운 스테이지가 시작 (첫시작)
    private void OnStartNewStage(GameEvents.NewStageStart evt) {
        // 씬에 따라 다른 브금 재생
        switch (evt.newStage) 
        {
            case Stage.Happy:
                PlayBGM((int)BGMType.HappyBGM);
                Debug.Log("첫방문이후 재생");
                break;

            case Stage.Sorrow:
                PlayBGM((int)BGMType.SorrowBGM);
                break;
        
        }
    }

    // 기존 스테이지 끝나면 브금 멈춤
    private void OnEndStage(GameEvents.StageEnd evt) { 
        StopBGM();
    }


    // 브금 재생
    private void OnPlayBGM(GameEvents.PlayBGM evt) {
        PlayBGM((int)evt.type);
    }


    // 브금 중단
    private void OnStopBGM(GameEvents.StopBGM evt) {
        StopBGM();
    }

    // SFX 이벤트 발행
    private void OnPlaySFX(GameEvents.PlaySFX evt) {
        int sfxIdx = (int)evt.type;

        PlaySFX(sfxIdx);
    }

    private void OnPlayOnlySFX(GameEvents.PlayOnlySFX evt) {
        int sfxIdx = (int)evt.type;

        PlayOnlySFX(sfxIdx);
    }

    private void OnPlayLoopSFX(GameEvents.PlayLoopSFX evt) {
        int sfxIdx = (int)evt.type;

    }

    // SFX 중단
    private void OnStopSFX(GameEvents.StopSFX evt) {
        StopSFX();
    }



    // BGM 재생
    private void PlayBGM(int bgmIndex) {
        if (bgmIndex < 0 || bgmIndex >= bgmClips.Length) return;
        if (isBGMPlaying && bgmSource.clip == bgmClips[bgmIndex]) return;     // 이미 같은 브금이 재생중이면 무시

        if (!isBGMPlaying)
        {
            StartCoroutine(BGMFadeIn(bgmClips[bgmIndex]));
        }
        // 이미 재생되고있는 브금이 있으면
        else
        {
            Debug.Log("Bug : 브금 겹침");

            if (bgmCoroutine != null) StopCoroutine(bgmCoroutine);

            bgmCoroutine = StartCoroutine(SwitchBGM(bgmClips[bgmIndex]));
        }
    }


    // BGM 정지
    private void StopBGM()
    {
        if (bgmCoroutine != null)
        {
            StopCoroutine(bgmCoroutine);
            bgmCoroutine = null;
        }
        bgmSource.clip = null;
        StartCoroutine(BGMFadeOut());
    }



    // SFX 재생
    public void PlaySFX(int sfxIndex)
    {
        if (sfxIndex < 0 || sfxIndex >= sfxClips.Length) return;

        if (!sfxSource01.isPlaying)
        {
            // 1번 SFX 플레이어가 비어있으면
            sfxSource01.PlayOneShot(sfxClips[sfxIndex]);
        }
        else if (!sfxSource02.isPlaying)
        {
            // 1번 사용중이면 2번에서 재생
            sfxSource02.PlayOneShot(sfxClips[sfxIndex]);
        }
        else {
            // 두 SFX 소스가 모두 사용중일 때
            Debug.LogError("3개의 소리가 겹쳐 첫번째 SFX 소리가 무시됨!!!!!!!!!!");
            // 첫번째 소스 무시하고 재생
            sfxSource01.PlayOneShot(sfxClips[sfxIndex]);
        }
    }

    // SFX 중복없이 재생
    public void PlayOnlySFX(int sfxIndex) {
        if (!isOnlySfxPlaying) {
            isOnlySfxPlaying = true;

            // 사운드 재생
            if (!sfxSource01.isPlaying)
            {
                sfxSource01.PlayOneShot(sfxClips[sfxIndex]);
            }
            else if (!sfxSource02.isPlaying)
            {
                sfxSource02.PlayOneShot(sfxClips[sfxIndex]);
            }
            else
            {
                Debug.LogError("3개의 소리가 겹쳐 첫번째 SFX 소리가 무시됨!!!!!!!!!!");
                sfxSource01.PlayOneShot(sfxClips[sfxIndex]);
            }

            // 끝나고 플래그 리셋
            StartCoroutine(ResetOnlySFX(sfxClips[sfxIndex].length));
        }
    }

    // Loop될 SFX 재생
    public void PlayLoopSFX(int sfxIndex) {
        if (!loopSfxSource.isPlaying) {
            loopSfxSource.clip = sfxClips[sfxIndex];
            loopSfxSource.loop = true;
            loopSfxSource.Play();
        }
    }


    // SFX 즉시 종료
    public void StopSFX() {
        sfxSource01.Stop();
        sfxSource02.Stop();
        loopSfxSource.Stop();

        isOnlySfxPlaying = false;
        isSFXPlaying = false;
    }


    // 코루틴
    public IEnumerator ResetOnlySFX(float delay) {
        yield return new WaitForSeconds(delay);

        isOnlySfxPlaying = false;
    }


    // 브금 페이드인
    public IEnumerator BGMFadeIn(AudioClip clip) {
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.volume = 0f;
        bgmSource.Play();

        // 유저 볼륨은 믹서에서 적용. 소스 볼륨은 페이드 전용
        float setVol = 1f;
        float time = 0f;

        while (time < fadeDuration) { 
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0, setVol, time / fadeDuration);
            yield return null;
        }

        bgmSource.volume = setVol;
        isBGMPlaying = true;
    }

    // 브금 페이드아웃
    public IEnumerator BGMFadeOut() {
        float startVol = bgmSource.volume;
        float time = 0f;

        while (time < fadeDuration) {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVol, 0f, time / fadeDuration);
            yield return null;
        }

        bgmSource.volume = 0f;
        bgmSource.Stop();
        isBGMPlaying = false;
    }

    // 만약, 브금이 겹쳤을 경우 브금 바꾸는 함수
    private IEnumerator SwitchBGM(AudioClip clip) {
        yield return BGMFadeOut();

        yield return BGMFadeIn(clip);
        bgmCoroutine = null;
    }

    // 오디오믹서의 현재 브금값 가져오기
    public float GetBGMVol()
    {
        float BGMVol;
        bool res = am.GetFloat("BGMVol", out BGMVol);
        if (res)
        {
            return Mathf.Pow(10, BGMVol / 20f);
        }
        else return 1f;
    }


    private static float LinearToMixerDb(float linear)
    {
        if (linear <= 0f) return -80f;
        return Mathf.Log10(linear) * 20f;
    }

    // Windows PlayerPrefs는 0f를 int 0으로 저장해서,
    // GetFloat가 기본값 1.0(100%)을 돌려주는 문제가 있음
    private const float MuteVolSentinel = -1f;
    private const float MissingFloatSentinel = -999f;

    private static float ReadVolumePref(string key, float defaultLinear)
    {
        if (!PlayerPrefs.HasKey(key))
            return defaultLinear;

        float stored = PlayerPrefs.GetFloat(key, MissingFloatSentinel);
        if (stored == MissingFloatSentinel || stored < 0f)
            return 0f;

        return Mathf.Clamp01(stored);
    }

    private static void SaveVolumePref(string key, float linear)
    {
        PlayerPrefs.SetFloat(key, linear <= 0f ? MuteVolSentinel : linear);
    }

    public float GetLastAudibleMasterVol() => ReadVolumePref(MasterVolLastKey, 1f);
    public float GetLastAudibleBgmVol() => ReadVolumePref(BGMVolLastKey, 1f);
    public float GetLastAudibleSfxVol() => ReadVolumePref(SFXVolLastKey, 1f);

    // Pref에 설정값 저장
    public void SetMasterVol(float val)
    {
        masterVol = val;
        am.SetFloat("MasterVol", LinearToMixerDb(val));
        SaveVolumePref(MasterVolKey, val);
        if (val > 0f)
            SaveVolumePref(MasterVolLastKey, val);
        PlayerPrefs.Save();
    }

    public void SetBGMVol(float val) {
        bgmVol = val;
        am.SetFloat("BGMVol", LinearToMixerDb(val));
        SaveVolumePref(BGMVolKey, val);
        if (val > 0f)
            SaveVolumePref(BGMVolLastKey, val);
        PlayerPrefs.Save();
    }

    public void SetSFXVol(float val)
    {
        sfxVol = val;
        am.SetFloat("SFXVol", LinearToMixerDb(val));
        SaveVolumePref(SFXVolKey, val);
        if (val > 0f)
            SaveVolumePref(SFXVolLastKey, val);
        PlayerPrefs.Save();
    }

    // 로드
    private void LoadVolumeSettings()
    {
        masterVol = ReadVolumePref(MasterVolKey, 1f);
        bgmVol = ReadVolumePref(BGMVolKey, 1f);
        sfxVol = ReadVolumePref(SFXVolKey, 1f);
    }

    private void ApplyMixerVolumes()
    {
        if (am == null) return;
        am.SetFloat("MasterVol", LinearToMixerDb(masterVol));
        am.SetFloat("BGMVol", LinearToMixerDb(bgmVol));
        am.SetFloat("SFXVol", LinearToMixerDb(sfxVol));
    }
}
