using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Flower : NeutralNPC
{
    [Header("Animator Ref")]
    [SerializeField] public Animator bodyAnim;
    [SerializeField] public Animator eyeAnim;

    [Header("EyeDirection")]
    [SerializeField] private Sprite[] eyes;

    [Header("Eye's SpriteRenderer")]
    [SerializeField] private SpriteRenderer sr;


    private Transform player;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        eyeAnim.enabled = false;
    }

    protected override void Update()
    {
        base.Update();

        if (player == null || sr == null || eyes == null || eyes.Length < 5) return;

        if (!isFriend)
        {
            UpdateEyeDirection();
            Debug.Log("들어오나?");
        }
    }

    private void UpdateEyeDirection()
    {
        // x좌표 차이 계산
        float diffX = player.position.x - transform.position.x;

        // 간단히 구간 나누기
        int eyeIndex;

        if (diffX < -2f)
            eyeIndex = 0; // 왼쪽 끝
        else if (diffX < -0.8f)
            eyeIndex = 1; // 덜 왼쪽
        else if (diffX < 0.8f)
            eyeIndex = 2; // 중간
        else if (diffX < 2f)
            eyeIndex = 3; // 덜 오른쪽
        else
            eyeIndex = 4; // 오른쪽 끝

        sr.sprite = eyes[eyeIndex];
        Debug.Log(eyeIndex);
    }

    protected override void OnEndNpcDialog(UIEvents.EndNpcDialog evt)
    {
        base.OnEndNpcDialog(evt);

        if (evt.npcID == npcID)
        {
            // 친화적이면 눈을 감음
            if (isFriend)
            {
                Debug.Log("당신은 꽃과 친화적입니다");
                eyeAnim.enabled = true;
                eyeAnim.SetTrigger("isRight");
            }
            else {
                Debug.Log("당신은 꽃과 적대적입니다");
                // 꽃 상태 유지
            }
        }

    }
}
