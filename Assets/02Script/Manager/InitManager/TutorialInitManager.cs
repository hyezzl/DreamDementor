using System.Linq;
using UnityEngine;

/// <summary>
/// TutorialScene 내 DB 주입 매니저
/// </summary>

public class TutorialInitManager : InitManager
{
    [Header("Tutorial 내 스크립트")]
    [SerializeField] private IntroEvent intro;
    [SerializeField] private TutorialEvent tutorial;
    [SerializeField] private TutorialTimeline timeline;
    [SerializeField] private TutorialDead tutoDead;

    protected override void Start()
    {
        base.Start();

        // Tutorial Map
        // Intro 이벤트
        if (intro != null)
        {
            intro.Init(db);
        }

        // 튜토리얼 이벤트 (컷씬)
        if (tutorial != null) {
            tutorial.Init(db);
        }

        // 튜토리얼 내 타임라인
        if (timeline != null) {
            timeline.Init(db);
        }

        // 튜토리얼 데드씬
        if (tutoDead != null) {
            tutoDead.Init(db);
        }
    }
}
