using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 일러스트 감상 시, 스파인 비활성화
/// </summary>

public class SpineManager : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic spine;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.SceneCover>(OnCover);
        EventBus.Instance.Subscribe<UIEvents.SceneDiscover>(OnDiscover);

        EventBus.Instance.Subscribe<GameEvents.OnLackedHP>(LackedHP);
        EventBus.Instance.Subscribe<GameEvents.OnSteadyHp>(InSteadyHP);

        UpdateSkin();
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.SceneCover>(OnCover);
        EventBus.Instance.Unsubscribe<UIEvents.SceneDiscover>(OnDiscover);

        EventBus.Instance.Unsubscribe<GameEvents.OnLackedHP>(LackedHP);
        EventBus.Instance.Unsubscribe<GameEvents.OnSteadyHp>(InSteadyHP);
    }


    // 일러스트 등장
    private void OnCover(UIEvents.SceneCover evt) {
        spine.gameObject.SetActive(false);
    }

    private void OnDiscover(UIEvents.SceneDiscover evt) {
        spine.gameObject.SetActive(true);
    }


    // 피 30이하일 때 스킨 바꿔주기
    private void LackedHP(GameEvents.OnLackedHP evt) {
        spine.Skeleton.SetSkin("Scar");
        spine.Skeleton.SetSlotsToSetupPose();
        spine.Update();
    }

    // 피가 안정되면 스킨 재변경
    private void InSteadyHP(GameEvents.OnSteadyHp evt) {
        spine.Skeleton.SetSkin("Nomal");
        spine.Skeleton.SetSlotsToSetupPose();
        spine.Update();
    }

    private void UpdateSkin() {
        int curHP = PlayerController.Instance.CurHP;
        if (curHP <= 30)
        {
            spine.Skeleton.SetSkin("Scar");
        }
        else
        {
            spine.Skeleton.SetSkin("Nomal");
        }
        spine.Skeleton.SetSlotsToSetupPose();
        spine.Update();
    }
}
