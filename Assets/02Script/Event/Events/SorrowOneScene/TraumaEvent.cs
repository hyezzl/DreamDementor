using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// SorrowOne 귀신 등장 이벤트
/// </summary>
public class TraumaEvent : TriggerZone
{
    public Animator enemyAnim;       // 괴물 애니메이터

    public override void OnTrigger(GameObject actor)
    {
        Debug.Log("부딪힘");
        //if (!isContacted && initialDialog != null)
        if (!isContacted)
        {
            // 귀신 등장음
            EventBus.Instance.Publish<GameEvents.PlaySFX>(new GameEvents.PlaySFX(SFXType.enemyLaugh));

            // 귀신 활성화
            enemyAnim.gameObject.SetActive(true);

            // 이벤트 모드 변경
            PlayerController.Instance.CurMode = GameMode.EventMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

            // 카메라 블렌딩 (타임라인? 단순 이벤트?)
            StartCoroutine(CameraBlend());

            // 대화이벤트
            Debug.Log("대화이벤트 진행!");
            //EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));
        }
    }


    // 대화 끝난 후
    public override void OnEndDialog(UIEvents.EndDialog evt)
    {
        // 귀신 사라짐
        StartCoroutine(FadeOutEnemy());

        // 마지막에!
        base.OnEndDialog(evt);
    }

    private IEnumerator CameraBlend() {
        EventBus.Instance.Publish<GameEvents.CameraShift>(new GameEvents.CameraShift(CameraType.EnemyCam, 1));
        
        yield return new WaitForSeconds(3f);    // 블렌드 + 쳐다보기

        EventBus.Instance.Publish<GameEvents.CameraShift>(new GameEvents.CameraShift(CameraType.PlayerSightCam, 1));
    }

    // 귀신 사라짐
    private IEnumerator FadeOutEnemy() {
        // 귀신 사라짐
        enemyAnim.SetTrigger("FadeOut");

        // 애니메이터에서 현재 상태 길이만큼 대기
        AnimatorStateInfo stateInfo = enemyAnim.GetCurrentAnimatorStateInfo(0);
        yield return null;
        stateInfo = enemyAnim.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSeconds(stateInfo.length);

        // 비활성화
        enemyAnim.gameObject.SetActive(false);
    }
}
