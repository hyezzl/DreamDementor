using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TempAnim
{ 
    Idle,
    gooseflesh,
    gooseflesh_Idle,
    CloseEye2,
    Lips,
}

public class TempSpineAnimation : MonoBehaviour
{

    public SkeletonGraphic skeleton;
    public string temp = "Close Eye 2";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayAnim(TempAnim.Idle.ToString(), true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayAnim(TempAnim.gooseflesh.ToString(), false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayAnim(TempAnim.gooseflesh_Idle.ToString(), true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayAnim(TempAnim.Lips.ToString(), true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayAnim(temp, true);
        }
    }

    private void PlayAnim(string name, bool loop = true) {
        if (skeleton != null) {
            skeleton.AnimationState.SetAnimation(0, name, loop);
        }
    }
}
