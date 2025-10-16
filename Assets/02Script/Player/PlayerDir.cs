using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬이동이나, 특정 이벤트 시 플레이어의 방향과 애니메이터 강제
/// </summary>
/// 

public enum Direction4
{ 
    Forward,    
    Back,
    Right,
    Left,
}



public class PlayerDir : MonoBehaviour
{
    private Animator anim;
    private PlayerMove pm;

    private void Awake()
    {
        if (!TryGetComponent<Animator>(out anim)) Debug.Log("PlayerDir - Failed to Load Animator");
        if (!TryGetComponent<PlayerMove>(out pm)) Debug.Log("PlayerDir - Failed to Load PlayerMove");
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.ForceDir>(OnForceDir);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.ForceDir>(OnForceDir);
    }

    private void OnForceDir(GameEvents.ForceDir evt) {
        pm.PreDir = ConverseDir(evt.dir);

        Vector3 localDir = transform.InverseTransformDirection(ConverseDir(evt.dir));
        anim.SetFloat("lookX", localDir.x);
        anim.SetFloat("lookY", localDir.z);

        anim.SetBool("isWalk", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("moveable", true);
    }

    // 4방 변환 함수
    public Vector3 ConverseDir(Direction4 dir) {
        switch (dir) {
            case Direction4.Forward:
                return Vector3.forward;
            case Direction4.Back:
                return Vector3.back;
            case Direction4.Right:
                return Vector3.right;
            case Direction4.Left:
                return Vector3.left;
            default:
                return Vector3.forward;
        }
    }
}
