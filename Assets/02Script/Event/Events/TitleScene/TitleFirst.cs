using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleFirst : MonoBehaviour
{

    public SkeletonGraphic spine;

    void Start()
    {
        if (spine != null) {
            spine.AnimationState.SetAnimation(0, "gooseflesh_Idle", true);
        }

        // ºê±Ý
        EventBus.Instance.Publish<GameEvents.PlayBGM>(new GameEvents.PlayBGM(BGMType.TitleBGM));
    }
}
