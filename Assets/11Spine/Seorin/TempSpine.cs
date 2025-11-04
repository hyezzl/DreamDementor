using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSpine : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic sg;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) {
            sg.Skeleton.SetSkin("Nomal");
            sg.Skeleton.SetSlotsToSetupPose();
            sg.Update();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            sg.Skeleton.SetSkin("Scar");
            sg.Skeleton.SetSlotsToSetupPose();
            sg.Update();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (sg != null)
            {
                sg.AnimationState.SetAnimation(0, PlayerEmotion.Idle.ToString(), true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (sg != null)
            {
                sg.AnimationState.SetAnimation(0, PlayerEmotion.Sad.ToString(), true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (sg != null)
            {
                sg.AnimationState.SetAnimation(0, PlayerEmotion.Sad_noteardrop.ToString(), true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (sg != null)
            {
                sg.AnimationState.SetAnimation(0, PlayerEmotion.Sad_smile.ToString(), true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (sg != null)
            {
                sg.AnimationState.SetAnimation(0, PlayerEmotion.Sad_smile_noteardrop.ToString(), true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (sg != null)
            {
                sg.AnimationState.SetAnimation(0, PlayerEmotion.Flustered_Idle.ToString(), true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (sg != null)
            {
                sg.AnimationState.SetAnimation(0, PlayerEmotion.Gooseflesh_Idle.ToString(), true);
            }
        }
    }
}
