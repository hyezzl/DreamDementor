using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static PuzzleEvents;

public class H_DistributionBox : CustomItem
{
    [SerializeField] private PuzzleType type;
    [SerializeField] private GameObject lights;

    [Header("Timeline02")]
    [SerializeField] private PlayableDirector timeline02;
    [SerializeField] private GameObject enemy;

    private bool isPass = false;    

    public override void OnEnable()
    {
        base.OnEnable();
        EventBus.Instance.Subscribe<PuzzleEvents.PassMinigame>(OnPassGame);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        EventBus.Instance.Unsubscribe<PuzzleEvents.PassMinigame>(OnPassGame);
    }

    private void Start()
    {
        if (lights != null)
            lights.SetActive(false);

        if (enemy != null)
            enemy.SetActive(false);
    }

    public override void Interact()
    {
        if (isPass) return;

        EventBus.Instance.Publish<UIEvents.OpenPuzzle>(new UIEvents.OpenPuzzle(type));
    }

    public void OnPassGame(PuzzleEvents.PassMinigame evt)
    {
        if (evt.puzzle == type && !isPass)
        {
            isPass = true;
            ChangeTag();

            //연출시작
            StartCoroutine(SequenceAfterPass());
        }
    }

    private IEnumerator SequenceAfterPass()
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        // 강제로 이벤트 모드 고정 (플레이어 멈춤)
        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish(new GameEvents.GameModeChange(GameMode.EventMode));

        if (lights != null) lights.SetActive(true);
        if (enemy != null) enemy.SetActive(true);

        // 플레이어위치..

        yield return StartCoroutine(PlayTimeline());
    }

    private IEnumerator PlayTimeline()
    {
        if (timeline02 == null) yield break;
        
        timeline02.Play();

        while (timeline02.state == PlayState.Playing) { 
            yield return null;
        }

        // 타임라인 끝나면
        PlayerController.Instance.CurMode = GameMode.InspectMode;
        EventBus.Instance.Publish(new GameEvents.GameModeChange(GameMode.InspectMode));

        EventBus.Instance.Publish(new GameEvents.EnemyStateChange(EnemyState.Patrol));
    }
}
